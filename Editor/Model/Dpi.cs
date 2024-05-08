namespace Editor.Model
{
    public record struct Dpi(string Display, string Value);

    public static class DpiConstants
    {
        public static IReadOnlyCollection<Dpi> All =>
        [
            new Dpi("6dpmm (150 dpi)","6"),
            new Dpi("8dpmm (203 dpi)","8"),
            new Dpi("12dpmm (300 dpi)","12"),
            new Dpi("24dpmm (600 dpi)","24"),
        ];
    }
}
