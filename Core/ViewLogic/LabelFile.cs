using Core.Interfaces;

namespace Core.ViewLogic
{
    public class LabelFile(string path) : IFile
    {
        public string Path => path;
        public string Name => System.IO.Path.GetFileName(path);
    }
}
