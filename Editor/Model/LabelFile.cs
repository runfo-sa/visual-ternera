namespace Editor.Model
{
    public class LabelFile(string Path)
    {
        public string Path { get; } = Path;
        public string Name => System.IO.Path.GetFileName(Path);
    }
}
