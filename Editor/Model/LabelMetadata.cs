using System.Runtime.Serialization;

namespace Editor.Model
{
    public class LabelMetadata
    {
        public Language? Language { get; set; }
    }

    public record struct Language(LanguageType LanguageType, char Letter)
    {
        public readonly string ParseContent(string content)
        {
            return LanguageType switch
            {
                LanguageType.Chinese | LanguageType.Japanese | LanguageType.Korean => content.Replace(
                    $"^A{Letter}",
                    $"^A{(char)LanguageType.Chinese}",
                    StringComparison.CurrentCultureIgnoreCase
                ),
                LanguageType.Cyrillic | LanguageType.Greek => content.Replace(
                    $"^A{Letter}",
                    $"^A{(char)LanguageType.Cyrillic}",
                    StringComparison.CurrentCultureIgnoreCase
                ),
                LanguageType.Arabic => content.Replace(
                    $"^A{Letter}",
                    $"^A{(char)LanguageType.Arabic}",
                    StringComparison.CurrentCultureIgnoreCase
                ),
                _ => content
            };
        }
    }

    public enum LanguageType
    {
        [EnumMember(Value = "Chinese")]
        Chinese = 'J',

        [EnumMember(Value = "Japanese")]
        Japanese = 'J',

        [EnumMember(Value = "Korean")]
        Korean = 'J',

        [EnumMember(Value = "Cyrillic")]
        Cyrillic = 'N',

        [EnumMember(Value = "Greek")]
        Greek = 'N',

        [EnumMember(Value = "Arabic")]
        Arabic = 'L'
    }
}
