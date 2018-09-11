using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IOptionSetV1TestDataCreator
    {
        IOptionSetV1TestDataCreator WithElement(XElement element);

        XDocument Build();
    }
}