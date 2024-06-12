namespace Core.Git
{
    public record struct GitTag(string Tag, string Date, string Message)
    {
        public static GitTag Parse(string input)
        {
            string[] inputs = input.Split('|', StringSplitOptions.TrimEntries);
            return new GitTag(inputs[0], inputs[1], inputs[2]);
        }

        public static GitTag Local = new("Local", DateTime.Now.ToString(), "Cambios locales");
    }
}
