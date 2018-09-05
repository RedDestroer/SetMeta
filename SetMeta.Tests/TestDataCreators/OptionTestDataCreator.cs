using System.Xml.Linq;
using SetMeta.Entities;

namespace SetMeta.Tests.TestDataCreators
{
    public class OptionTestDataCreator
        : IOptionTestDataCreator
    {
        private string _displayName;
        private string _description;
        private string _defaultValue;
        private bool _asAttribute;
        private OptionValueType _valueType;
        private XElement _behaviourElement;
        private XElement _element;
        private XAttribute _attribute;

        public IOptionTestDataCreator WithDisplayName(string displayName)
        {
            _displayName = displayName;

            return this;
        }

        public IOptionTestDataCreator WithDescription(string description)
        {
            _description = description;

            return this;
        }

        public IOptionTestDataCreator WithDefaultValue(string defaultValue, bool asAttribute = true)
        {
            _defaultValue = defaultValue;
            _asAttribute = asAttribute;

            return this;
        }

        public IOptionTestDataCreator WithValueType(OptionValueType valueType)
        {
            _valueType = valueType;

            return this;
        }

        public IOptionTestDataCreator WithBehaviour(XElement behaviourElement)
        {
            _behaviourElement = behaviourElement;

            return this;
        }

        public IOptionTestDataCreator WithAttribute(XAttribute attribute)
        {
            _attribute = attribute;

            return this;
        }

        public IOptionTestDataCreator WithElement(XElement element)
        {
            _element = element;

            return this;
        }

        public XElement Build(string name)
        {
            var element = new XElement(Keys.Option, new XAttribute(OptionAttributeKeys.Name, name));

            if (_attribute != null)
                element.Add(_attribute);

            if (_displayName != null)
                element.Add(new XAttribute(OptionAttributeKeys.DisplayName, _displayName));
            if (_description != null)
                element.Add(new XAttribute(OptionAttributeKeys.Description, _description));

            element.Add(new XAttribute(OptionAttributeKeys.ValueType, _valueType));

            if (_defaultValue != null)
            {
                if (_asAttribute)
                {
                    element.Add(new XAttribute(OptionAttributeKeys.DisplayName, _defaultValue));
                }
                else
                {
                    var defaultValueElement = new XElement(OptionAttributeKeys.DisplayName, new XCData(_defaultValue));
                    element.Add(defaultValueElement);
                }
            }

            if (_behaviourElement != null)
                element.Add(_behaviourElement);

            if (_element != null)
                element.Add(_element);

            return element;
        }
    }
}