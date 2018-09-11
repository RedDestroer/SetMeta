using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Util;
using FixedListElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.FixedListElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class FixedListBehaviourTestDataCreator
        : IFixedListBehaviourTestDataCreator
    {
        private readonly IList<XElement> _listItems = new List<XElement>();
        private IListItemTestDataCreator _listItemTestDataCreator;

        public FixedListBehaviourTestDataCreator()
        {
            ListItemTestDataCreator = new ListItemTestDataCreator();
        }

        public IListItemTestDataCreator ListItemTestDataCreator
        {
            get => _listItemTestDataCreator;
            set => _listItemTestDataCreator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IFixedListBehaviourTestDataCreator WithListItems(IEnumerable<XElement> elements)
        {
            _listItems.AddRange(elements);

            return this;
        }

        public IFixedListBehaviourTestDataCreator WithListItem(XElement element)
        {
            _listItems.Add(element);

            return this;
        }

        public IFixedListBehaviourTestDataCreator WithListItem(string value = null, string displayValue = null)
        {
            _listItems.Add(ListItemTestDataCreator.WithValue(value).WithDisplayValue(displayValue).Build());

            return this;
        }

        public XElement Build()
        {
            var body = new XElement(FixedListElement.ElementName);

            foreach (var listItem in _listItems)
            {
                body.Add(listItem);
            }

            return body;
        }
    }
}