using System.Collections.Generic;
using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IMultiListBehaviourTestDataCreator
    {
        IMultiListBehaviourTestDataCreator AsSorted(bool? sorted = true);
        IMultiListBehaviourTestDataCreator WithSeparator(string separator);
        IMultiListBehaviourTestDataCreator WithListItems(IEnumerable<XElement> elements);
        IMultiListBehaviourTestDataCreator WithListItem(XElement element);
        IMultiListBehaviourTestDataCreator WithListItem(string value = null, string displayValue = null);
        XElement Build();
    }
}