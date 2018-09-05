using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Entities;

namespace SetMeta.Tests.TestDataCreators
{
    public class MultiListBehaviourTestDataCreator
        : IMultiListBehaviourTestDataCreator
    {
        private readonly IList<KeyValuePair<string, string>> _listItems = new List<KeyValuePair<string, string>>();
        private bool? _sorted;
        private string _separator;

        public IMultiListBehaviourTestDataCreator AsSorted()
        {
            _sorted = true;

            return this;
        }

        public IMultiListBehaviourTestDataCreator WithSeparator(string separator)
        {
            _separator = separator;

            return this;
        }

        public IMultiListBehaviourTestDataCreator WithListItem(string value, string displayValue)
        {
            _listItems.Add(new KeyValuePair<string, string>(value, displayValue));

            return this;
        }

        public XElement Build()
        {
            var body = new XElement(MultiListBehaviourKeys.Name);

            if (_sorted != null)
                body.Add(new XAttribute(MultiListBehaviourKeys.AttrKeys.Sorted, _sorted));
            if (_separator != null)
                body.Add(new XAttribute(MultiListBehaviourKeys.AttrKeys.Separator, _separator));

            foreach (var pair in _listItems)
            {
                var listItem = new XElement(ListItemKeys.Name, new XAttribute(ListItemKeys.AttrKeys.Value, pair.Key));
                if (pair.Value != null)
                    listItem.Add(new XAttribute(ListItemKeys.AttrKeys.DisplayValue, pair.Value));

                body.Add(listItem);
            }

            return body;
        }
    }
}