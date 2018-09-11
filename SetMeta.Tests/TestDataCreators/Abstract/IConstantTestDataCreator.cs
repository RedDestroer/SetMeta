using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IConstantTestDataCreator
    {
        IConstantTestDataCreator WithValue(string value);
        XElement Build(string name);
    }
}