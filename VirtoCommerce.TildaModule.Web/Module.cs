using Microsoft.Practices.Unity;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.TildaModule.Web.Services;

namespace VirtoCommerce.ContentModule.Web
{
    public class Module : ModuleBase
    {
        private readonly IUnityContainer _container;

        public Module(IUnityContainer container)
        {
            _container = container;
        }

        #region Public Methods and Operators

        public override void Initialize()
        {
            _container.RegisterType<TildaPublishingService, TildaPublishingService>();
        }

        public override void PostInitialize()
        {
            base.PostInitialize();
        }

        #endregion
    }
}
