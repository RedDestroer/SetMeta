using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Entities;

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
            var body = new XElement(FlagListBehaviourKeys.Name);

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