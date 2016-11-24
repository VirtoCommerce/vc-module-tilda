using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TildaModule.Web.Model;

namespace VirtoCommerce.TildaModule.Web.Services
{
    public class TildaPublishingService
    {
        private readonly string _publicKey;
        private readonly string _privateKey;
        private readonly string _url = "http://api.tildacdn.info";
        private readonly IContentBlobStorageProvider _storageProvider;

        public TildaPublishingService(IContentBlobStorageProvider storageProvider, ISettingsManager settings)
        {
            _privateKey = settings.GetValue("VirtoCommece.Tilda.PublicKey", string.Empty);
            _publicKey = settings.GetValue("VirtoCommece.Tilda.PrivateKey", string.Empty);
            _storageProvider = storageProvider;
        }

        public string PublicKey
        {
            get
            {
                return _publicKey;
            }
        }

        public TildaPage GetPageBody(int pageId)
        {
            return RequestTildaApi<TildaPageResponse>($"{_url}/v1/getpage/?publickey={_publicKey}&secretkey={_privateKey}&pageid={pageId}").Result;
        }

        public TildaPage GetFullPageExport(int pageId)
        {
            return RequestTildaApi<TildaPageResponse>($"{_url}/v1/getpagefullexport/?publickey={_publicKey}&secretkey={_privateKey}&pageid={pageId}").Result;
        }

        public TildaProject GetProject(int projectId)
        {
            return RequestTildaApi<TildaProjectResponse>($"{_url}/v1/getproject/?publickey={_publicKey}&secretkey={_privateKey}&projectid={projectId}").Result;
        }

        public TildaPage[] GetPages(int projectId)
        {
            return RequestTildaApi<TildaPageListResponse>($"{_url}/v1/getpageslist/?publickey={_publicKey}&secretkey={_privateKey}&projectid={projectId}").Result;
        }

        public void SavePage(string storeId, TildaPage page)
        {
            using (var targetStream = _storageProvider.OpenWrite($"Pages/{storeId}/{page.FileName}"))
            {
                var sw = new StreamWriter(targetStream);
                sw.Write(page.Html);
            }
        }

        public void SaveProject(string storeId, TildaProject project)
        {
            using (var targetStream = _storageProvider.OpenWrite($"Themes/{storeId}/tilda-css-js.liquid"))
            {
                var sw = new StreamWriter(targetStream);
                foreach(var css in project.Css)
                {
                    	//<script src="/js/tilda-scripts-2.6.min.js"  type="text/javascript"></script>
                    sw.Write($"<link href=\"{css}\" rel=\"stylesheet\" media=\"screen\">");
                }

                foreach (var js in project.Js)
                {
                    sw.Write($"<script src=\"{js}\" type=\"text/javascript\"></script>");
                }

                sw.Flush();
            }
        }

        private T RequestTildaApi<T>(string url)
        {
            T result;
            // Call the service
            var address = new Uri(url);
            var request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "text/json";
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Get the stream containing content returned by the server.
                    var newStream = response.GetResponseStream();
                    // Open the stream using a StreamReader for easy access.
                    using (var reader = new StreamReader(newStream))
                    {
                        result = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                    }
                }
            }
            catch (WebException ex)
            {
                throw;
            }

            return result;
        }
    }
}