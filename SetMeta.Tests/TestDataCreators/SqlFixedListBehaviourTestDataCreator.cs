using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using SqlFixedListElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.SqlFixedListElement;

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
            var body = new XElement(SqlFixedListElement.ElementName, new XAttribute(SqlFixedListElement.Attrs.Query, query));

            if (_valueFieldName != null)
                body.Add(new XAttribute(SqlFixedListElement.Attrs.ValueFieldName, _valueFieldName));
            if (_displayValueFieldName != null)
                body.Add(new XAttribute(SqlFixedListElement.Attrs.DisplayValueFieldName, _displayValueFieldName));

            return body;
        }
    }
}