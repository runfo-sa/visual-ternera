using System.IO;
using System.Xml.Serialization;

namespace Editor.Model
{
    public record struct LabelSize(string Display, string Value);

    public class SizeList
    {
        public LabelSize[] Items;
    }

    public static class SizeConstants
    {
        public static LabelSize[] GetList(string content)
        {
            XmlSerializer serializer = new(typeof(SizeList));
            using (StringReader reader = new(content))
            {
                var list = (SizeList)serializer.Deserialize(reader)!;
                return list.Items;
            }
        }
    }
}
