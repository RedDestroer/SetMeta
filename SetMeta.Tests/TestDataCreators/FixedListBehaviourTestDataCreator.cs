using System.Collections.Generic;
using System.Xml.Linq;
using FixedListElement = SetMeta.Entities.OptionSetElement.OptionElement.FixedListElement;
using ListItemElement = SetMeta.Entities.OptionSetElement.OptionElement.FixedListElement.ListItemElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class FixedListBehaviourTestDataCreator
        : IFixedListBehaviourTestDataCreator
    {
        private readonly IList<KeyValuePair<string, string>> _listItems = new List<KeyValuePair<string, string>>();

        public IFixedListBehaviourTestDataCreator WithListItem(string value, string displayValue = null)
        {
            _listItems.Add(new KeyValuePair<string, string>(value, displayValue));

            return this;
        }

        public XElement Build()
        {
            var body = new XElement(FixedListElement.ElementName);

            foreach (var pair in _listItems)
            {
                var listItem = new XElement(ListItemElement.ElementName, new XAttribute(ListItemElement.Attrs.Value, pair.Key));
                if (pair.Value != null)
                    listItem.Add(new XAttribute(ListItemElement.Attrs.DisplayValue, pair.Value));

                body.Add(listItem);
            }

            return body;
        }
    }
}