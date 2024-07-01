using System.Runtime.Serialization;

namespace Core.Services.LabelaryModel
{
    public enum LabelaryLanguageType
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
