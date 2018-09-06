using System.Xml.Linq;
using SqlMultiListElement = SetMeta.Entities.OptionSetElement.OptionElement.SqlMultiListElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class SqlMultiListBehaviourTestDataCreator
        : ISqlMultiListBehaviourTestDataCreator
    {
        private string _valueFieldName;
        private string _displayValueFieldName;
        private bool? _sorted;
        private string _separator;

        public ISqlMultiListBehaviourTestDataCreator WithValueFieldName(string valueFieldName)
        {
            _valueFieldName = valueFieldName;

            return this;
        }

        public ISqlMultiListBehaviourTestDataCreator WithDisplayValueFieldName(string displayValueFieldName)
        {
            _displayValueFieldName = displayValueFieldName;

            return this;
        }

        public ISqlMultiListBehaviourTestDataCreator AsSorted()
        {
            _sorted = true;

            return this;
        }

        public ISqlMultiListBehaviourTestDataCreator WithSeparator(string separator)
        {
            _separator = separator;

            return this;
        }

        public XElement Build(string query)
        {
            var body = new XElement(SqlMultiListElement.ElementName, new XAttribute(SqlMultiListElement.Attrs.Query, query));

            if (_sorted != null)
                body.Add(new XAttribute(SqlMultiListElement.Attrs.Sorted, _sorted));
            if (_separator != null)
                body.Add(new XAttribute(SqlMultiListElement.Attrs.Separator, _separator));
            if (_valueFieldName != null)
                body.Add(new XAttribute(SqlMultiListElement.Attrs.ValueFieldName, _valueFieldName));
            if (_displayValueFieldName != null)
                body.Add(new XAttribute(SqlMultiListElement.Attrs.DisplayValueFieldName, _displayValueFieldName));

            return body;
        }
    }
}