using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using ListItemElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.FixedListElement.ListItemElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class ListItemTestDataCreator
        : IListItemTestDataCreator
    {
        private string _value;
        private string _displayValue;

        public IListItemTestDataCreator WithValue(string value)
        {
            _value = value;

            return this;
        }

        public IListItemTestDataCreator WithDisplayValue(string displayValue)
        {
            _displayValue = displayValue;

            return this;
        }

        public XElement Build()
        {
            var element = new XElement(ListItemElement.ElementName);

            if (_value != null)
                element.Add(new XAttribute(ListItemElement.Attrs.Value, _value));
            if (_displayValue != null)
                element.Add(new XAttribute(ListItemElement.Attrs.DisplayValue, _displayValue));

            return element;
        }
    }
}