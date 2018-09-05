using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IFixedListBehaviourTestDataCreator
    {
        IFixedListBehaviourTestDataCreator WithListItem(string value, string displayValue = null);
        XElement Build();
    }
}