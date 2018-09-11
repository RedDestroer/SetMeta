using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IOptionSetV1TestDataCreator
    {
        IOptionSetV1TestDataCreator WithElement(XElement element);

        XDocument Build();
    }
}