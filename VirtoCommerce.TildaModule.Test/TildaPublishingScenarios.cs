using Moq;
using System;
using System.IO;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.TildaModule.Web.Services;
using Xunit;

namespace VirtoCommerce.TildaModule.Test
{
    public class TildaPublishingScenarios
    {
        [Fact]
        public void Tilda_can_receive_page_content()
        {
            var storage = GetStorageProvider();
            var service = new TildaPublishingService(storage, GetSettingsManager());
            var page = service.GetPageBody(200337);
            Console.Write(page.Html);
            service.SavePage("sample", page);
        }

        [Fact]
        public void Tilda_can_sync_content()
        {
            var storage = GetStorageProvider();
            var service = new TildaPublishingService(storage, GetSettingsManager());
            var project = service.GetProject(60994);
            service.SaveProject("sample", project);
            var pages = service.GetPages(60994);
            foreach(var page in pages)
            {
                var pageFull = service.GetPageBody(page.Id);
                Console.Write(pageFull.Html);
                service.SavePage("sample", pageFull);
            }
        }

        private ISettingsManager GetSettingsManager()
        {
            var mock = new Mock<ISettingsManager>();
            var publicKey = Environment.GetEnvironmentVariable("Tilda_PublicKey");
            var privateKey = Environment.GetEnvironmentVariable("Tilda_PrivateKey");
            mock.Setup(s => s.GetValue("VirtoCommece.Tilda.PublicKey", "")).Returns(publicKey);
            mock.Setup(s => s.GetValue("VirtoCommece.Tilda.PrivateKey", "")).Returns(privateKey);
            return mock.Object;
        }

        private IContentBlobStorageProvider GetStorageProvider()
        {
            return new FileSystemContentBlobStorageProvider(Path.GetTempPath() + "tilda-test", "http://localhost/files");
        }
    }
}
