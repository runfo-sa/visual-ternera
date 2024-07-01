namespace Editor.Services
{
    public class EditorPreviewMediator : IEditorPreviewMediator
    {
        private readonly CompositeCommand _generatePreview = new();
        public CompositeCommand GeneratePreview => _generatePreview;

        private readonly CompositeCommand _sendErrors = new();
        public CompositeCommand SendErrors => _sendErrors;
    }
}
