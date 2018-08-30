using System.Collections.Generic;
using System.Xml.Linq;
using SetMeta.Entities;
using SetMeta.Util;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IOptionSetParserV1TestDataCreator
    {
        XElement WithOption(string name, string displayName = null, string description = null, string defaultValue = null, string valueType = null, XElement innerElement = null);
        XDocument Build();
    }

    public class OptionSetParserV1TestDataCreator
        : IOptionSetParserV1TestDataCreator
    {
        private readonly IList<XElement> _options;

        public OptionSetParserV1TestDataCreator()
        {
            _options = new List<XElement>();
        }

        public XElement WithOption(string name, string displayName = null, string description = null, string defaultValue = null, string valueType = null, XElement innerElement = null)
        {
            var option = new XElement(Keys.Option);
            option.Add(new XAttribute(OptionAttributeKeys.Name, name));
            if (displayName != null)
                option.Add(new XAttribute(OptionAttributeKeys.DisplayName, displayName));
            if (description != null)
                option.Add(new XAttribute(OptionAttributeKeys.Description, description));
            if (defaultValue != null)
                option.Add(new XAttribute(OptionAttributeKeys.DefaultValue, defaultValue));
            if (valueType != null)
                option.Add(new XAttribute(OptionAttributeKeys.ValueType, valueType));
            if (innerElement != null)
                option.Add(innerElement);

            _options.Add(option);

            return option;
        }

        public XDocument Build()
        {
            var body = new XDocument();

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            _options.ForEach<XElement>(element => body.Add(element));

            return body;
        }
    }
}