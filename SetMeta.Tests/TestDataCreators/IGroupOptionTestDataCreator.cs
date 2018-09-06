using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IGroupOptionTestDataCreator
    {
        IGroupOptionTestDataCreator WithGroupOption(XElement element);
        XElement Build(string name);
    }
}