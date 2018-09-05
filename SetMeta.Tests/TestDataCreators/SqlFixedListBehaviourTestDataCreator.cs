using System.Xml.Linq;
using SetMeta.Entities;

namespace SetMeta.Tests.TestDataCreators
{
    public class SqlFixedListBehaviourTestDataCreator
        : ISqlFixedListBehaviourTestDataCreator
    {
        private string _valueFieldName;
        private string _displayValueFieldName;

        public ISqlFixedListBehaviourTestDataCreator WithValueFieldName(string valueFieldName)
        {
            _valueFieldName = valueFieldName;

            return this;
        }

        public ISqlFixedListBehaviourTestDataCreator WithDisplayValueFieldName(string displayValueFieldName)
        {
            _displayValueFieldName = displayValueFieldName;

            return this;
        }

        public XElement Build(string query)
        {
            var body = new XElement(SqlFixedListBehaviourKeys.Name, new XAttribute(SqlFixedListBehaviourKeys.AttrKeys.Query, query));

            if (_valueFieldName != null)
                body.Add(new XAttribute(SqlFixedListBehaviourKeys.AttrKeys.ValueFieldName, _valueFieldName));
            if (_displayValueFieldName != null)
                body.Add(new XAttribute(SqlFixedListBehaviourKeys.AttrKeys.DisplayValueFieldName, _displayValueFieldName));

            return body;
        }
    }
}