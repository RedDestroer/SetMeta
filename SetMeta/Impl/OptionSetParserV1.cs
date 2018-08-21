using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using SetMeta.Abstract;
using SetMeta.Behaviours;
using SetMeta.Entities;
using SetMeta.Util;

namespace SetMeta.Impl
{
    internal class OptionSetParserV1
        : OptionSetParser
    {
        private readonly IOptionValueFactory _optionValueFactory;

        public OptionSetParserV1(IOptionValueFactory optionValueFactory)
        {
            _optionValueFactory = optionValueFactory ?? throw new ArgumentNullException(nameof(optionValueFactory));
        }

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

            foreach (var element in body.Elements(Keys.Option))
            {
                optionSet.Options.Add(ParseOption(element));
            }

            return optionSet;
        }

        private Option ParseOption(XElement root)
        {
            var option = new Option();           

            option.Name = root.GetAttributeValue<string>(OptionAttributeKeys.Name);
            option.DisplayName = root.TryGetAttributeValue(OptionAttributeKeys.DisplayName, OptionAttributeDefaults.DisplayName);
            option.Description = root.TryGetAttributeValue(OptionAttributeKeys.Description, OptionAttributeDefaults.Description);
            option.DefaultValue = root.TryGetAttributeValue(OptionAttributeKeys.DefaultValue, OptionAttributeDefaults.DefaultValue);
            option.ValueType = root.TryGetAttributeValue(OptionAttributeKeys.ValueType, OptionAttributeDefaults.ValueType);
            var optionValue = _optionValueFactory.Create(option.ValueType);
            option.Behaviour = CreateBehaviour(root, optionValue);

            return option;
        }

        private OptionBehaviour CreateBehaviour(XElement root, IOptionValue optionValue)
        {
            var elements = root.Elements();

            foreach (var element in elements)
            {
                if (TryCreateBehaviour(element, optionValue, out var optionBehaviour))
                {
                    return optionBehaviour;
                }
            }
           
            return new SimpleOptionBehaviour(optionValue);
        }

        private bool TryCreateBehaviour(XElement root, IOptionValue optionValue, out OptionBehaviour optionBehaviour)
        {
            string name = root.Name.LocalName;
            optionBehaviour = null;
            string max = null;
            string min = null;
            string query = null;
            string valueFieldName = null;
            string displayValueFieldName = null;
            bool sorted = false;
            string separator = null;

            switch (name)
            {
                case "rangedMinMax":
                    min = root.GetAttributeValue<string>("min");
                    max = root.GetAttributeValue<string>("max");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, min, max);
                    break;
                case "rangedMax":
                    max = root.GetAttributeValue<string>("max");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, max, false);
                    break;
                case "rangedMin":
                    min = root.GetAttributeValue<string>("min");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, min, true);
                    break;
                case "fixedList":
                    optionBehaviour = CreateFixedListBehaviour(root, optionValue);
                    break;
                case "flagList":
                    optionBehaviour = CreateFlagListBehaviour(root, optionValue);
                    break;
                case "multiList":
                    sorted = root.GetAttributeValue<bool>("sorted");
                    separator = root.GetAttributeValue<string>("separator");
                    optionBehaviour = CreateMultiListBehaviour(root, optionValue, sorted, separator);
                    break;
                case "sqlFixedList":
                    query = root.GetAttributeValue<string>("query");
                    valueFieldName = root.GetAttributeValue<string>("valueFieldName");
                    displayValueFieldName = root.GetAttributeValue<string>("displayValueFieldName");
                    optionBehaviour = new SqlFixedListOptionBehaviour(optionValue, query,valueFieldName, displayValueFieldName);
                    break;
                case "sqlFlagList":
                    query = root.GetAttributeValue<string>("query");
                    valueFieldName = root.GetAttributeValue<string>("valueFieldName");
                    displayValueFieldName = root.GetAttributeValue<string>("displayValueFieldName");
                    optionBehaviour = new SqlFlagListOptionBehaviour(optionValue, query, valueFieldName, displayValueFieldName);
                    break;
                case "sqlMultiList":
                    query = root.GetAttributeValue<string>("query");
                    sorted = root.GetAttributeValue<bool>("sorted");
                    separator = root.GetAttributeValue<string>("separator");
                    valueFieldName = root.GetAttributeValue<string>("valueFieldName");
                    displayValueFieldName = root.GetAttributeValue<string>("displayValueFieldName");
                    optionBehaviour = new SqlMultiListOptionBehaviour(optionValue, query, sorted, separator, valueFieldName, displayValueFieldName);
                    break;
            }

            return optionBehaviour != null;
        }

        private List<ListItem> CreateBehaviourList(XElement root, IOptionValue optionValue)
        {
            var elements = root.Elements();
            var list = new List<ListItem>();

            foreach (var element in elements)
            {
                if (element.Name.LocalName == "listItem")
                {
                    var value = element.GetAttributeValue<string>("value");
                    var displayValue = element.GetAttributeValue<string>("displayValue");
                    list.Add(new ListItem{Value = value, DisplayValue = displayValue});
                }
            }

            return list;
        }

        private FixedListOptionBehaviour CreateFixedListBehaviour(XElement root, IOptionValue optionValue)
        {
            var list = CreateBehaviourList(root, optionValue);

            return new FixedListOptionBehaviour(optionValue, list);
        }

        private FlagListOptionBehaviour CreateFlagListBehaviour(XElement root, IOptionValue optionValue)
        {
            var list = CreateBehaviourList(root, optionValue);

            return new FlagListOptionBehaviour(optionValue, list);
        }

        private MultiListOptionBehaviour CreateMultiListBehaviour(XElement root, IOptionValue optionValue, bool sorted, string separator)
        {
            var list = CreateBehaviourList(root, optionValue);

            return new MultiListOptionBehaviour(optionValue, list, sorted, separator);
        }
    }
}
