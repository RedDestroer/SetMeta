using System.Xml.Linq;
using SetMeta.Entities;

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
            var body = new XElement(SqlMultiListBehaviourKeys.Name, new XAttribute(SqlMultiListBehaviourKeys.AttrKeys.Query, query));

            if (_sorted != null)
                body.Add(new XAttribute(SqlMultiListBehaviourKeys.AttrKeys.Sorted, _sorted));
            if (_separator != null)
                body.Add(new XAttribute(SqlMultiListBehaviourKeys.AttrKeys.Separator, _separator));
            if (_valueFieldName != null)
                body.Add(new XAttribute(SqlMultiListBehaviourKeys.AttrKeys.ValueFieldName, _valueFieldName));
            if (_displayValueFieldName != null)
                body.Add(new XAttribute(SqlMultiListBehaviourKeys.AttrKeys.DisplayValueFieldName, _displayValueFieldName));

            return body;
        }
    }
}