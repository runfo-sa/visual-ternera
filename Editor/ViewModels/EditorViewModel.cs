using Editor.Services;

namespace Editor.ViewModels
{
    public class EditorViewModel : BindableBase
    {
        public EditorViewModel(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterScoped<ICommandService, CommandsService>();
            containerRegistry.RegisterScoped<IEditorPreviewMediator, EditorPreviewMediator>();
        }
    }
}
