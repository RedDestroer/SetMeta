using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IGroupTestDataCreator
    {
        IGroupTestDataCreator WithElement(XElement element);
        IGroupTestDataCreator WithDisplayName(string displayName);
        IGroupTestDataCreator WithDescription(string description);
        XElement Build(string name);
    }
}