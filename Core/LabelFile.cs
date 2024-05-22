namespace Core
{
    public class LabelFile(string path)
    {
        public string Path => path;
        public string Name => System.IO.Path.GetFileName(path).ToLowerInvariant();
    }
}
