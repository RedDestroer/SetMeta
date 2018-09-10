using System.Collections.Generic;
using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IFixedListBehaviourTestDataCreator
    {
        IFixedListBehaviourTestDataCreator WithListItems(IEnumerable<XElement> elements);
        IFixedListBehaviourTestDataCreator WithListItem(XElement element);
        IFixedListBehaviourTestDataCreator WithListItem(string value = null, string displayValue = null);
        XElement Build();
    }
}