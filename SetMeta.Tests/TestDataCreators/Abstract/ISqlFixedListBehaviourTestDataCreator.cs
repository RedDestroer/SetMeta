using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface ISqlFixedListBehaviourTestDataCreator
    {
        ISqlFixedListBehaviourTestDataCreator WithValueFieldName(string valueFieldName);
        ISqlFixedListBehaviourTestDataCreator WithDisplayValueFieldName(string displayValueFieldName);
        XElement Build(string query);
    }
}