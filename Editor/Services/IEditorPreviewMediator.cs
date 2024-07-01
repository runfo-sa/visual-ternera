namespace Editor.Services
{
    public interface IEditorPreviewMediator
    {
        CompositeCommand GeneratePreview { get; }
        CompositeCommand SendErrors { get; }
    }
}
