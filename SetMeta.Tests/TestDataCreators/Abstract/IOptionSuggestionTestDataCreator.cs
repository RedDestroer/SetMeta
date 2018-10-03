using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IOptionSuggestionTestDataCreator
    {
        XElement Build(string name);
    }
}