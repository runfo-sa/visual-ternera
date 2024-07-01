using AdonisUI.Controls;
using Core.Services;
using Core.View;
using Editor.Views;

namespace Editor
{
    [Module(ModuleName = "Editor#FileEdit", OnDemand = true)]
    public class EditorModule(IRegionManager regionManager) : IModule
    {
        private readonly IRegionManager _regionManager = regionManager;
        private IContainerProvider? _container;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _container = containerProvider;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(new ModuleLoaderService("Editor", CreateWindow));
            containerRegistry.RegisterDialog<About>();

            _regionManager.RegisterViewWithRegion("MenuRegion", typeof(Menu));
            _regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(Toolbar));
            _regionManager.RegisterViewWithRegion("TreeRegion", typeof(Tree));
            _regionManager.RegisterViewWithRegion("TextEditorRegion", typeof(TextEditor));
            _regionManager.RegisterViewWithRegion("PreviewRegion", typeof(Preview));
        }

        private void CreateWindow(string name)
        {
            new AdonisWindow
            {
                Title = $"Visual Ternera - {name}",
                Content = _container?.Resolve<Views.Editor>()
            }.Show();
        }
    }
}
