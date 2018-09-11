using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface ISqlMultiListBehaviourTestDataCreator
    {
        ISqlMultiListBehaviourTestDataCreator AsSorted(bool? sorted = false);
        ISqlMultiListBehaviourTestDataCreator WithSeparator(string separator);
        ISqlMultiListBehaviourTestDataCreator WithValueFieldName(string valueFieldName);
        ISqlMultiListBehaviourTestDataCreator WithDisplayValueFieldName(string displayValueFieldName);
        XElement Build(string query);
    }
}