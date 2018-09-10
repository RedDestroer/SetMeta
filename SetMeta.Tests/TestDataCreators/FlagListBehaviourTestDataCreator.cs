using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Util;
using FlagListElement = SetMeta.Entities.OptionSetElement.OptionElement.FlagListElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class FlagListBehaviourTestDataCreator
        : IFlagListBehaviourTestDataCreator
    {
        private readonly IList<XElement> _listItems = new List<XElement>();
        private IListItemTestDataCreator _listItemTestDataCreator;

        public FlagListBehaviourTestDataCreator()
        {
            ListItemTestDataCreator = new ListItemTestDataCreator();
        }

        public IListItemTestDataCreator ListItemTestDataCreator
        {
            get => _listItemTestDataCreator;
            set => _listItemTestDataCreator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IFlagListBehaviourTestDataCreator WithListItems(IEnumerable<XElement> elements)
        {
            _listItems.AddRange(elements);

            return this;
        }

        public IFlagListBehaviourTestDataCreator WithListItem(XElement element)
        {
            _listItems.Add(element);

            return this;
        }

        public IFlagListBehaviourTestDataCreator WithListItem(string value = null, string displayValue = null)
        {
            _listItems.Add(ListItemTestDataCreator.WithValue(value).WithDisplayValue(displayValue).Build());

            return this;
        }

        public XElement Build()
        {
            var body = new XElement(FlagListElement.ElementName);

            foreach (var listItem in _listItems)
            {
                body.Add(listItem);
            }

            return body;
        }
    }
}