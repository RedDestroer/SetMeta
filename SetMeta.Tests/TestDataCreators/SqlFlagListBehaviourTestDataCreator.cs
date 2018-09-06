using System.Xml.Linq;
using SqlFlagListElement = SetMeta.Entities.OptionSetElement.OptionElement.SqlFlagListElement;

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
            var body = new XElement(SqlFlagListElement.ElementName, new XAttribute(SqlFlagListElement.Attrs.Query, query));

            if (_valueFieldName != null)
                body.Add(new XAttribute(SqlFlagListElement.Attrs.ValueFieldName, _valueFieldName));
            if (_displayValueFieldName != null)
                body.Add(new XAttribute(SqlFlagListElement.Attrs.DisplayValueFieldName, _displayValueFieldName));

            return body;
        }
    }
}