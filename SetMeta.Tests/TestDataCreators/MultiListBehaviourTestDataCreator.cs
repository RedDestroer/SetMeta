using System;
using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using SetMeta.Util;
using MultiListElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.MultiListElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class MultiListBehaviourTestDataCreator
        : IMultiListBehaviourTestDataCreator
    {
        private readonly IList<XElement> _listItems = new List<XElement>();
        private IListItemTestDataCreator _listItemTestDataCreator;
        private bool? _sorted;
        private string _separator;

        public MultiListBehaviourTestDataCreator()
        {
            ListItemTestDataCreator = new ListItemTestDataCreator();
        }

        public IListItemTestDataCreator ListItemTestDataCreator
        {
            get => _listItemTestDataCreator;
            set => _listItemTestDataCreator = value ?? throw new ArgumentNullException(nameof(value));
        }

        public IMultiListBehaviourTestDataCreator AsSorted(bool? sorted = true)
        {
            _sorted = sorted;

            return this;
        }

        public IMultiListBehaviourTestDataCreator WithSeparator(string separator)
        {
            _separator = separator;

            return this;
        }

        public IMultiListBehaviourTestDataCreator WithListItems(IEnumerable<XElement> elements)
        {
            _listItems.AddRange(elements);

            return this;
        }

        public IMultiListBehaviourTestDataCreator WithListItem(XElement element)
        {
            _listItems.Add(element);

            return this;
        }

        public IMultiListBehaviourTestDataCreator WithListItem(string value = null, string displayValue = null)
        {
            _listItems.Add(ListItemTestDataCreator.WithValue(value).WithDisplayValue(displayValue).Build());

            return this;
        }

        public XElement Build()
        {
            var body = new XElement(MultiListElement.ElementName);

            if (_sorted != null)
                body.Add(new XAttribute(MultiListElement.Attrs.Sorted, _sorted));
            if (_separator != null)
                body.Add(new XAttribute(MultiListElement.Attrs.Separator, _separator));

            foreach (var listItem in _listItems)
            {
                body.Add(listItem);
            }

            return body;
        }
    }
}