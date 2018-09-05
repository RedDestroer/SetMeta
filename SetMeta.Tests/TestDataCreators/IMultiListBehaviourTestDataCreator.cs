using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IMultiListBehaviourTestDataCreator
    {
        IMultiListBehaviourTestDataCreator AsSorted();
        IMultiListBehaviourTestDataCreator WithSeparator(string separator);
        IMultiListBehaviourTestDataCreator WithListItem(string value, string displayValue);
        XElement Build();
    }
}