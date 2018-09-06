using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IConstantTestDataCreator
    {
        XElement Build(string name, string value, string valueType);
    }
}