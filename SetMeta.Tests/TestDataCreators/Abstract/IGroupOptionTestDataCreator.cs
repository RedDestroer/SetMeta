using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IGroupOptionTestDataCreator
    {
        IGroupOptionTestDataCreator WithGroupOption(XElement element);
        XElement Build(string name);
    }
}