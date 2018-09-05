using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface ISqlFlagListBehaviourTestDataCreator
    {
        ISqlFlagListBehaviourTestDataCreator WithValueFieldName(string valueFieldName);
        ISqlFlagListBehaviourTestDataCreator WithDisplayValueFieldName(string displayValueFieldName);
        XElement Build(string query);
    }
}