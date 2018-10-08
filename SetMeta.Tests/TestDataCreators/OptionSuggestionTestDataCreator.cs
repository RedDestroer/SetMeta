using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using SuggestionElement = SetMeta.XmlKeys.OptionSetElement.GroupElement.OptionElement.SuggestionElement;
using NamedSuggestionElement = SetMeta.XmlKeys.OptionSetElement.SuggestionElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class OptionSuggestionTestDataCreator
        : IOptionSuggestionTestDataCreator
    {
        private readonly IList<XElement> _innerElements;

        public OptionSuggestionTestDataCreator()
        {
            _innerElements = new List<XElement>();
        }

        public IOptionSuggestionTestDataCreator WithMinLength(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MinLengthElement.ElementName, new XAttribute(NamedSuggestionElement.MinLengthElement.Attrs.Value, length.ToString())));

            return this;
        }

        public IOptionSuggestionTestDataCreator WithMaxLength(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MaxLengthElement.ElementName, new XAttribute(NamedSuggestionElement.MaxLengthElement.Attrs.Value, length.ToString())));

            return this;
        }

        public IOptionSuggestionTestDataCreator WithMinLines(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MinLinesElement.ElementName, new XAttribute(NamedSuggestionElement.MinLinesElement.Attrs.Value, length.ToString())));

            return this;
        }

        public IOptionSuggestionTestDataCreator WithMaxLines(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MaxLinesElement.ElementName, new XAttribute(NamedSuggestionElement.MaxLinesElement.Attrs.Value, length.ToString())));

            return this;
        }

        public IOptionSuggestionTestDataCreator WithMultiLine()
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MultilineElement.ElementName));

            return this;
        }

        public IOptionSuggestionTestDataCreator WithNotifiable()
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.NotifiableElement.ElementName));

            return this;
        }

        public IOptionSuggestionTestDataCreator WithNotifyOnChange()
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.NotifyOnChangeElement.ElementName));

            return this;
        }

        public IOptionSuggestionTestDataCreator WithRegex(string value, string validation)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.RegexElement.ElementName, new XAttribute(NamedSuggestionElement.RegexElement.Attrs.Value, value), new XAttribute(NamedSuggestionElement.RegexElement.Attrs.Validation, validation)));

            return this;
        }

        public XElement Build(string name)
        {
            var element = new XElement(SuggestionElement.ElementName, new XAttribute(SuggestionElement.Attrs.Name, name));

            foreach (var innerElement in _innerElements)
            {
                element.Add(innerElement);
            }

            return element;
        }
    }
}