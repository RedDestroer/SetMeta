using System.Collections.Generic;
using System.Xml.Linq;
using FlagListElement = SetMeta.Entities.OptionSetElement.OptionElement.FlagListElement;
using ListItemElement = SetMeta.Entities.OptionSetElement.OptionElement.FlagListElement.ListItemElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class FlagListBehaviourTestDataCreator
        : IFlagListBehaviourTestDataCreator
    {
        private readonly IList<KeyValuePair<string, string>> _listItems = new List<KeyValuePair<string, string>>();

        public IFlagListBehaviourTestDataCreator WithListItem(string value, string displayValue = null)
        {
            _listItems.Add(new KeyValuePair<string, string>(value, displayValue));

            return this;
        }

        public XElement Build()
        {
            var body = new XElement(FlagListElement.ElementName);

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