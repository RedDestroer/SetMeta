using System.Xml.Linq;
using SetMeta.Entities;

namespace SetMeta.Tests.TestDataCreators
{
    public class SqlFlagListBehaviourTestDataCreator
        : ISqlFlagListBehaviourTestDataCreator
    {
        private string _valueFieldName;
        private string _displayValueFieldName;

        public ISqlFlagListBehaviourTestDataCreator WithValueFieldName(string valueFieldName)
        {
            _valueFieldName = valueFieldName;

            return this;
        }

        public ISqlFlagListBehaviourTestDataCreator WithDisplayValueFieldName(string displayValueFieldName)
        {
            _displayValueFieldName = displayValueFieldName;

            return this;
        }

        public XElement Build(string query)
        {
            var body = new XElement(SqlFlagListBehaviourKeys.Name, new XAttribute(SqlFlagListBehaviourKeys.AttrKeys.Query, query));

            if (_valueFieldName != null)
                body.Add(new XAttribute(SqlFlagListBehaviourKeys.AttrKeys.ValueFieldName, _valueFieldName));
            if (_displayValueFieldName != null)
                body.Add(new XAttribute(SqlFlagListBehaviourKeys.AttrKeys.DisplayValueFieldName, _displayValueFieldName));

            return body;
        }
    }
}