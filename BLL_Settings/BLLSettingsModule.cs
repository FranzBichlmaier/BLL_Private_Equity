using BLL_Infrastructure;
using BLL_Settings.Views;
using Microsoft.Practices.Unity;

using Prism.Modularity;


namespace BLL_Settings
{
    public class BLLSettingsModule : IModule
    {
        private readonly IUnityContainer container;

        public BLLSettingsModule(IUnityContainer container)
        {
            this.container = container;
        }
        public void Initialize()
        {
            container.RegisterType<object, ChangeFontSizes>(ViewNames.ChangeFontSizes);
        }
    }
}
