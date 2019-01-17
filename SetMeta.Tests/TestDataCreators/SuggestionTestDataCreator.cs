using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using NamedSuggestionElement = SetMeta.XmlKeys.OptionSetElement.SuggestionElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class SuggestionTestDataCreator
        : ISuggestionTestDataCreator
    {
        private readonly IList<XElement> _innerElements;

        public SuggestionTestDataCreator()
        {
            _innerElements = new List<XElement>();
        }

        public ISuggestionTestDataCreator WithMinLength(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MinLengthElement.ElementName, new XAttribute(NamedSuggestionElement.MinLengthElement.Attrs.Value, length)));

            return this;
        }

        public ISuggestionTestDataCreator WithMaxLength(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MaxLengthElement.ElementName, new XAttribute(NamedSuggestionElement.MaxLengthElement.Attrs.Value, length)));

            return this;
        }

        public ISuggestionTestDataCreator WithMinLines(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MinLinesElement.ElementName, new XAttribute(NamedSuggestionElement.MinLinesElement.Attrs.Value, length)));

            return this;
        }

        public ISuggestionTestDataCreator WithMaxLines(string length)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MaxLinesElement.ElementName, new XAttribute(NamedSuggestionElement.MaxLinesElement.Attrs.Value, length)));

            return this;
        }

        public ISuggestionTestDataCreator WithMultiline()
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.MultilineElement.ElementName));

            return this;
        }

        public ISuggestionTestDataCreator WithNotifiable()
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.NotifiableElement.ElementName));

            return this;
        }

        public ISuggestionTestDataCreator WithNotifyOnChange()
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.NotifyOnChangeElement.ElementName));

            return this;
        }

        public ISuggestionTestDataCreator WithRegex(string value, string validation)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.RegexElement.ElementName, new XAttribute(NamedSuggestionElement.RegexElement.Attrs.Value, value), new XAttribute(NamedSuggestionElement.RegexElement.Attrs.Validation, validation)));

            return this;
        }

        /// <inheritdoc />
        public ISuggestionTestDataCreator WithControl(string value)
        {
            _innerElements.Add(new XElement(NamedSuggestionElement.ControlElement.ElementName, new XAttribute(NamedSuggestionElement.ControlElement.Attrs.Value, value)));

            return this;
        }

        public XElement Build(string name)
        {
            var element = new XElement(NamedSuggestionElement.ElementName, new XAttribute(NamedSuggestionElement.Attrs.Name, name));

            foreach (var innerElement in _innerElements)
            {
                element.Add(innerElement);
            }

            return element;
        }
    }
}