using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IFlagListBehaviourTestDataCreator
    {
        IFlagListBehaviourTestDataCreator WithListItem(string value, string displayValue = null);
        XElement Build();
    }
}