using AdonisUI.Controls;
using Core.Services;

namespace TestModule
{
    [Module(ModuleName = "Test Module", OnDemand = true)]
    public class TestModuleModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider) { }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(new ModuleLoaderService("Test Module", CreateWindow));
        }

        private void CreateWindow(string name)
        {
            new AdonisWindow
            {
                Title = $"Visual Ternera - {name}",
                Content = new Views.ViewTest(),
                DataContext = new ViewModels.TestViewModel()
            }.Show();
        }
    }
}
