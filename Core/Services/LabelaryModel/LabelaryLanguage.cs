namespace Core.Services.LabelaryModel
{
    public record struct LabelaryLanguage(LabelaryLanguageType LanguageType, char Letter)
    {
        public readonly string ParseContent(string content)
        {
            return LanguageType switch
            {
                LabelaryLanguageType.Chinese | LabelaryLanguageType.Japanese | LabelaryLanguageType.Korean => content.Replace(
                    $"^A{Letter}",
                    $"^A{(char)LabelaryLanguageType.Chinese}",
                    StringComparison.CurrentCultureIgnoreCase
                ),
                LabelaryLanguageType.Cyrillic | LabelaryLanguageType.Greek => content.Replace(
                    $"^A{Letter}",
                    $"^A{(char)LabelaryLanguageType.Cyrillic}",
                    StringComparison.CurrentCultureIgnoreCase
                ),
                LabelaryLanguageType.Arabic => content.Replace(
                    $"^A{Letter}",
                    $"^A{(char)LabelaryLanguageType.Arabic}",
                    StringComparison.CurrentCultureIgnoreCase
                ),
                _ => content
            };
        }
    }
}
