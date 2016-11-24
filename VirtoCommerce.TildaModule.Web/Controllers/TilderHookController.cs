using Hangfire;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.TildaModule.Web.Model;
using VirtoCommerce.TildaModule.Web.Services;

namespace VirtoCommerce.TildaModule.Web.Controllers
{
    [RoutePrefix("api/tildapublishing/{store}")]
    public class TilderHookController : ApiController
    {
        private readonly IPushNotificationManager _notifier;
        private readonly IUserNameResolver _userNameResolver;
        private readonly TildaPublishingService _publishingService;

        public TilderHookController(
            IPushNotificationManager pushNotificationManager,
            IUserNameResolver userNameResolver,
            TildaPublishingService publishingService
            )
        {
            _notifier = pushNotificationManager;
            _userNameResolver = userNameResolver;
            _publishingService = publishingService;
        }

        [Route("webhook")]
        public IHttpActionResult TildaCallBack(string store, int pageId, int projectId, string published, string publicKey)
        {
            if (publicKey != _publishingService.PublicKey)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            var syncInfo = new SyncInfo() { PageId = pageId, ProjectId = projectId };
            return Sync(store, syncInfo);
            //return BadRequest();
        }

        /// <summary>
        /// Start catalog data export process.
        /// </summary>
        /// <remarks>Data export is an async process. An ExportNotification is returned for progress reporting.</remarks>
        /// <param name="syncInfo">The export configuration.</param>
        [HttpPost]
        [Route("export")]
        [ResponseType(typeof(SyncNotification))]
        public IHttpActionResult Sync(string store, SyncInfo syncInfo)
        {
            var notification = new SyncNotification(_userNameResolver.GetCurrentUserName())
            {
                Title = "Tilda Publishing Sync",
                Description = "starting syncing ..."
            };
            _notifier.Upsert(notification);


            BackgroundJob.Enqueue(() => BackgroundSync(store, syncInfo, notification));

            return Ok(notification);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // Only public methods can be invoked in the background. (Hangfire)
        public void BackgroundSync(string store, SyncInfo syncInfo, SyncNotification notifyEvent)
        {
            try
            {
                var project = _publishingService.GetProject(syncInfo.ProjectId);
                _publishingService.SaveProject(store, project);
                var pages = _publishingService.GetPages(syncInfo.ProjectId);
                foreach (var page in pages)
                {
                    var pageFull = _publishingService.GetPageBody(page.Id);
                    Console.Write(pageFull.Html);
                    _publishingService.SavePage(store, pageFull);
                }

            }
            catch (Exception ex)
            {
                notifyEvent.Description = "Sync failed";
            }
            finally
            {
                //notifyEvent.Finished = DateTime.UtcNow;
                _notifier.Upsert(notifyEvent);
            }
        }
    }
}