using System.Xml.Linq;
using SuggestionElement = SetMeta.XmlKeys.OptionSetElement.GroupElement.OptionElement.SuggestionElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class OptionSuggestionTestDataCreator
        : IOptionSuggestionTestDataCreator
    {
        public XElement Build(string name)
        {
            var element = new XElement(SuggestionElement.ElementName, new XAttribute(SuggestionElement.Attrs.Name, name));

            return element;
        }
    }
}