using System.Collections.Generic;
using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IFlagListBehaviourTestDataCreator
    {
        IFlagListBehaviourTestDataCreator WithListItems(IEnumerable<XElement> elements);
        IFlagListBehaviourTestDataCreator WithListItem(XElement element);
        IFlagListBehaviourTestDataCreator WithListItem(string value = null, string displayValue = null);
        XElement Build();
    }
}