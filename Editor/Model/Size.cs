namespace Editor.Model
{
    public record struct Size(string Display, string Value);

    public static class SizeConstants
    {
        public static IReadOnlyCollection<Size> All =>
        [
            new Size("Primaria - [10x8cm]","4x3"),
            new Size("Caja - [10x15cm]","4x6")
        ];
    }
}
