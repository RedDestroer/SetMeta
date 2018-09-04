using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Entities;
using SetMeta.Util;

namespace SetMeta.Tests.TestDataCreators
{
    public interface ISetMetaTestDataCreator
    {
        IOptionSetV1TestDataCreator OptionSet { get; }
        IOptionTestDataCreator Option { get; }
        IRangedMinMaxBehaviourTestDataCreator RangedMinMaxBehaviour { get; }
        IRangedMaxBehaviourTestDataCreator RangedMaxBehaviour { get; }
        IRangedMinBehaviourTestDataCreator RangedMinBehaviour { get; }
    }

    public class SetMetaTestDataCreator
        : ISetMetaTestDataCreator
    {
        public SetMetaTestDataCreator()
        {
            OptionSet = new OptionSetV1TestDataCreator();
            Option = new OptionTestDataCreator();
            RangedMinMaxBehaviour = new RangedMinMaxBehaviourTestDataCreator();
            RangedMaxBehaviour = new RangedMaxBehaviourTestDataCreator();
            RangedMinBehaviour = new RangedMinBehaviourTestDataCreator();
        }

        public IOptionSetV1TestDataCreator OptionSet { get; }
        public IOptionTestDataCreator Option { get; }
        public IRangedMinMaxBehaviourTestDataCreator RangedMinMaxBehaviour { get; }
        public IRangedMaxBehaviourTestDataCreator RangedMaxBehaviour { get; }
        public IRangedMinBehaviourTestDataCreator RangedMinBehaviour { get; }
    }

    public interface IOptionSetV1TestDataCreator
    {
        IOptionSetV1TestDataCreator WithElement(XElement element);

        XDocument Build();
    }

    public static class OptionSetV1TestDataCreatorExtension
    {
        public static XDocument BuildNew(this IOptionSetV1TestDataCreator tdc, AutoFixtureBase autoFixture)
        {
            tdc.WithElement(autoFixture.TestDataCreator.Option.BuildNew(autoFixture));
            tdc.WithElement(autoFixture.TestDataCreator.Option.BuildNew(autoFixture));
            tdc.WithElement(autoFixture.TestDataCreator.Option.BuildNew(autoFixture));

            return tdc.Build();
        }

        public static XDocument BuildNew(this IOptionSetV1TestDataCreator tdc, XElement element)
        {
            tdc.WithElement(element);

            return tdc.Build();
        }
    }

    public class OptionSetV1TestDataCreator
        : IOptionSetV1TestDataCreator
    {
        private readonly IList<XElement> _elements;

        public OptionSetV1TestDataCreator()
        {
            _elements = new List<XElement>();
        }

        public IOptionSetV1TestDataCreator WithElement(XElement element)
        {
            _elements.Add(element);

            return this;
        }

        public XDocument Build()
        {
            var body = new XElement(Keys.OptionSet,
                new XAttribute(OptionSetAttributeKeys.Version, "1"));
            ////new XAttribute("xmlns", "http://tempuri.org")
            ////new XAttribute("xmlns:xsi", "http://tempuri.org/2018/Option/XMLSchema-instance"),
            ////new XAttribute("xsi:schemaLocation", "http://tempuri.org OptionSetV1.xsd"));

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            _elements.ForEach<XElement>(element => body.Add(element));

            var document = new XDocument();
            document.Add(body);

            return document;
        }
    }

    public interface IOptionTestDataCreator
    {
        IOptionTestDataCreator WithDisplayName(string displayName);
        IOptionTestDataCreator WithDescription(string description);
        IOptionTestDataCreator WithDefaultValue(string defaultValue, bool asElement = false);
        IOptionTestDataCreator WithValueType(OptionValueType valueType);
        IOptionTestDataCreator WithBehaviour(XElement behaviourElement);
        IOptionTestDataCreator WithAttribute(XAttribute attribute);
        IOptionTestDataCreator WithElement(XElement element);
        XElement Build(string name);
    }

    public static class OptionTestDataCreatorExtension
    {
        public static XElement BuildNew(this IOptionTestDataCreator tdc, AutoFixtureBase autoFixture)
        {
            tdc.WithDisplayName(autoFixture.Fake<string>());
            tdc.WithDescription(autoFixture.Fake<string>());
            tdc.WithValueType(autoFixture.Fake<OptionValueType>());

            return tdc.Build(autoFixture.Fake<string>());
        }

        public static XElement BuildNew(this IOptionTestDataCreator tdc, AutoFixtureBase autoFixture, XElement element)
        {
            tdc.WithValueType(autoFixture.Fake<OptionValueType>());
            tdc.WithElement(element);

            return tdc.Build(autoFixture.Fake<string>());
        }
    }

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

    public interface IRangedMinMaxBehaviourTestDataCreator
    {
        XElement Build(string min, string max);
    }

    public class RangedMinMaxBehaviourTestDataCreator
        : IRangedMinMaxBehaviourTestDataCreator
    {
        public XElement Build(string min, string max)
        {
            return new XElement(
                RangedMinMaxBehaviourKeys.Name, 
                new XAttribute(RangedMinMaxBehaviourKeys.AttrKeys.Min, min),
                new XAttribute(RangedMinMaxBehaviourKeys.AttrKeys.Max, max));
        }
    }

    public interface IRangedMaxBehaviourTestDataCreator
    {
        XElement Build(string max);
    }

    public class RangedMaxBehaviourTestDataCreator
        : IRangedMaxBehaviourTestDataCreator
    {
        public XElement Build(string max)
        {
            return new XElement(
                RangedMaxBehaviourKeys.Name,
                new XAttribute(RangedMaxBehaviourKeys.AttrKeys.Max, max));
        }
    }

    public interface IRangedMinBehaviourTestDataCreator
    {
        XElement Build(string min);
    }

    public class RangedMinBehaviourTestDataCreator
        : IRangedMinBehaviourTestDataCreator
    {
        public XElement Build(string min)
        {
            return new XElement(
                RangedMinBehaviourKeys.Name,
                new XAttribute(RangedMinBehaviourKeys.AttrKeys.Min, min));
        }
    }
}