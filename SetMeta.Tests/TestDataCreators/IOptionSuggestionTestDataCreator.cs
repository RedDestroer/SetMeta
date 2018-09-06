using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IOptionSuggestionTestDataCreator
    {
        XElement Build(string name);
    }
}