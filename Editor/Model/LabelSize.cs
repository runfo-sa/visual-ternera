namespace Editor.Model
{
    public record struct LabelSize(string Display, string Value);

    public static class SizeConstants
    {
        public static IReadOnlyCollection<LabelSize> All =>
        [
            new LabelSize("Primaria - [10x8cm]","4x3"),
            new LabelSize("Caja - [10x15cm]","4x6")
        ];
    }
}
