using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using SetMeta.Abstract;
using SetMeta.Dao;
using SetMeta.Util;

namespace SetMeta.Impl
{
    internal class OptionSetParserV1
        : OptionSetParser
    {
        public override string Version => "1";

        public override OptionSet Parse(XmlTextReader reader)
        {
            Validate.NotNull(reader, nameof(reader));

            var optionSet = new OptionSet();
            var document = XDocument.Load(reader);
            var body = document.Root;
            if (body == null)
                throw new InvalidOperationException("Xml body is absent.");

            optionSet.Version = Version;

            ParseOptions(body, optionSet.Options);

            return optionSet;
        }

        private void ParseOptions(XElement root, IList<Option> options)
        {
            root.Elements(Keys.Option)
                .ForEach(o => options.Add(ParseOption(o)));
        }

        private Option ParseOption(XElement root)
        {
            var option = new Option();           

            option.Name = root.GetAttributeValue<string>(OptionAttributeKeys.Name);
            option.DisplayName = root.TryGetAttributeValue(OptionAttributeKeys.DisplayName, OptionAttributeDefaults.DisplayName);
            option.Description = root.TryGetAttributeValue(OptionAttributeKeys.Description, OptionAttributeDefaults.Description);
            option.DefaultValue = root.TryGetAttributeValue(OptionAttributeKeys.DefaultValue, OptionAttributeDefaults.DefaultValue);
            option.ValueType = root.TryGetAttributeValue(OptionAttributeKeys.ValueType, OptionAttributeDefaults.ValueType);
            
            return option;
        }
    }
}
