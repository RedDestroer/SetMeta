using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SetMeta.Abstract;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Entities.Suggestions;
using SetMeta.Util;

namespace SetMeta.Impl
{
    internal class OptionSetParserV1
        : OptionSetParser
    {
        private readonly IOptionValueFactory _optionValueFactory;

        private IOptionSetValidator _optionSetValidator;

        public OptionSetParserV1(IOptionValueFactory optionValueFactory)
        {
            _optionValueFactory = optionValueFactory ?? throw new ArgumentNullException(nameof(optionValueFactory));
        }

        public override string Version => "1";

        public override OptionSet Parse(XmlTextReader reader, IOptionSetValidator optionSetValidator)
        {
            Validate.NotNull(reader, nameof(reader));
            Validate.NotNull(optionSetValidator, nameof(optionSetValidator));

            _optionSetValidator = optionSetValidator;

            var optionSet = new OptionSet();
            var document = XDocument.Load(reader);
            var body = document.Root;
            if (body == null)
                throw new InvalidOperationException("Xml body is absent.");

            optionSet.Version = Version;

            ParseConstants(body, optionSet.Constants);
            ParseOptions(body, optionSet.Options);
            ParseGroups(body, optionSet.Groups);

            return optionSet;
        }

        private void ParseConstants(XElement root, IDictionary<string, Constant> constants)
        {
            root.Elements(Keys.Constant)
                .ForEach(element =>
                {
                    var constant = ParseConstant(element);
                    if (KeyIsValid(constant.Name))
                    {
                        if (KeyIsUnique(constant.Name, constants))
                        {
                            constants[constant.Name] = constant;
                        }
                        else
                        {
                            _optionSetValidator.AddError($"Key '{constant.Name}' isn`t unique among constants.", element);
                        }
                    }
                    else
                    {
                        _optionSetValidator.AddError($"Key '{constant.Name}' ('{constant.Name}') isn`t valid.", element);
                    }
                });
        }

        private void ParseGroups(XElement root, IDictionary<string, Group> groups)
        {
            root.Elements(Keys.Group)
                .ForEach(element =>
                {
                    var group = ParseGroup(element);
                    if (KeyIsValid(group.Id))
                    {
                        if (KeyIsUnique(group.Id, groups))
                        {
                            groups[group.Id] = group;
                        }
                        else
                        {
                            _optionSetValidator.AddError($"Key '{group.Id}' isn`t unique among groups.", element);
                        }
                    }
                    else
                    {
                        _optionSetValidator.AddError($"Key '{group.Id}' ('{group.Name}') isn`t valid.", element);
                    }
                });
        }

        private Group ParseGroup(XElement root)
        {
            var group = new Group();

            group.Name = TryGetMandatoryAttributeValue<string>(root, GroupAttributeKeys.Name);
            group.Id = CreateId(group.Name);
            group.DisplayName = root.TryGetAttributeValue(GroupAttributeKeys.DisplayName, GroupAttributeDefaults.DisplayName);
            group.Description = root.TryGetAttributeValue(GroupAttributeKeys.Description, GroupAttributeDefaults.Description);
            ParseOptions(root, group.Options);
            ParseGroups(root, group.Groups);
            ParseSuggestions(root, group.Suggestions);

            return group;
        }

        private void ParseSuggestions(XElement root, IDictionary<string, IDictionary<SuggestionType, Suggestion>> suggestions)
        {
            root.Elements(Keys.Option)
                .ForEach(element =>
                {
                    var tempDictionary = new Dictionary<SuggestionType, Suggestion>();

                    element.Elements(Keys.Suggestion)
                        .ForEach(suggestionElement =>
                        {
                            var suggestion = CreateSuggestion(suggestionElement);

                            if (tempDictionary.ContainsKey(suggestion.SuggestionType))
                            {
                                string optionName = element.TryGetAttributeValue("name", "optionName");
                                _optionSetValidator.AddError($"Suggestion with type '{suggestion.SuggestionType}' isn`t unique among option '{optionName}'.", element);                              
                            }
                            else
                            {
                                tempDictionary.Add(suggestion.SuggestionType, suggestion);
                            }
                        });

                    string name = element.TryGetAttributeValue("name", "optionName");
                    string id = CreateId(name);

                    if (suggestions.ContainsKey(id))
                    {
                        _optionSetValidator.AddError($"Key '{id}' isn`t unique among options.", element);
                    }
                    else
                    {
                        suggestions.Add(id, tempDictionary);
                    }
                });
        }

        private Suggestion CreateSuggestion(XElement root)
        {
            var elements = root.Elements();

            foreach (var element in elements)
            {
                if (TryCreateSuggestion(element, out var suggestion))
                {
                    return suggestion;
                }
            }

            return null;
        }

        private bool TryCreateSuggestion(XElement root, out Suggestion suggestion)
        {
            string name = root.Name.LocalName;
            suggestion = null;

            switch (name)
            {
                case "maxLength":
                {
                    UInt16 max = root.GetAttributeValue<UInt16>("value");
                    suggestion = new MaxLengthSuggestion {Value = max};
                }
                    break;
                case "maxLines":
                {
                    byte max = root.GetAttributeValue<byte>("value");
                    suggestion = new MaxLinesSuggestion {Value = max};
                }
                    break;
                case "minLength":
                {
                    UInt16 min = root.GetAttributeValue<UInt16>("value");
                    suggestion = new MinLengthSuggestion { Value = min };
                }
                    break;
                case "minLines":
                {
                    byte min = root.GetAttributeValue<byte>("value");
                    suggestion = new MinLinesSuggestion { Value = min };
                }
                    break;
                case "multiline":
                {
                    suggestion = new MultiLineSuggestion();
                }
                    break;
                case "notifiable":
                {
                    suggestion = new NotifiableSuggestion();
                }
                    break;
                case "notifyOnChange":
                {
                    suggestion = new NotifyOnChangeSuggestion();
                }
                    break;
                case "regex":
                {
                    string value = root.GetAttributeValue<string>("value");
                    string validation = root.TryGetAttributeValue<string>("validation", null);
                    suggestion = new RegexSuggestion { Value = value, Validation = validation};
                }
                    break;
            }

            return suggestion != null;
        }

        private Constant ParseConstant(XElement root)
        {
            var constant = new Constant();

            constant.Name = TryGetMandatoryAttributeValue<string>(root, ConstantAttributeKeys.Name);
            constant.ValueType = TryGetMandatoryAttributeValue<string>(root, ConstantAttributeKeys.ValueType);
            constant.Value = TryGetMandatoryAttributeValue<string>(root, ConstantAttributeKeys.Value);

            return constant;
        }

        private void ParseOptions(XElement root, IDictionary<string, Option> options)
        {
            root.Elements(Keys.Option)
                .ForEach(element =>
                {
                    var option = ParseOption(element);
                    if (KeyIsValid(option.Id))
                    {
                        if (KeyIsUnique(option.Id, options))
                        {
                            options[option.Id] = option;
                        }
                        else
                        {
                            _optionSetValidator.AddError($"Key '{option.Id}' isn`t unique among options.", element);
                        }
                    }
                    else
                    {
                        _optionSetValidator.AddError($"Key '{option.Id}' ('{option.Name}') isn`t valid.", element);
                    }
                });
        }

        private bool KeyIsUnique(string id, IDictionary<string, Option> options)
        {
            return !options.ContainsKey(id);
        }

        private bool KeyIsUnique(string id, IDictionary<string, Group> groups)
        {
            return !groups.ContainsKey(id);
        }

        private bool KeyIsUnique(string id, IDictionary<string, Constant> constants)
        {
            return !constants.ContainsKey(id);
        }

        private bool KeyIsValid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            if (id.ToCharArray()
                .Any(o => !AllowedChars.ContainsKey(o)))
                return false;

            return true;
        }

        private Option ParseOption(XElement root)
        {
            var option = new Option();

            option.Name = TryGetMandatoryAttributeValue<string>(root, OptionAttributeKeys.Name);
            option.Id = CreateId(option.Name);
            option.DisplayName = root.TryGetAttributeValue(OptionAttributeKeys.DisplayName, OptionAttributeDefaults.DisplayName);
            option.Description = root.TryGetAttributeValue(OptionAttributeKeys.Description, OptionAttributeDefaults.Description);
            option.DefaultValue = root.TryGetAttributeValue(OptionAttributeKeys.DefaultValue, OptionAttributeDefaults.DefaultValue);
            option.ValueType = root.TryGetAttributeValue(OptionAttributeKeys.ValueType, OptionAttributeDefaults.ValueType);
            var optionValue = _optionValueFactory.Create(option.ValueType);
            option.Behaviour = CreateBehaviour(root, optionValue);

            return option;
        }

        private T TryGetMandatoryAttributeValue<T>(XElement root, string name)
        {
            if (!root.TryGetAttributeValue(name, out T value))
                _optionSetValidator.AddError($"Mandatory attribute {name} not found.", root);

            return value;
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
            
            switch (name)
            {
                case "rangedMinMax":
                {
                    string min = root.GetAttributeValue<string>("min");
                    string max = root.GetAttributeValue<string>("max");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, min, max);
                }
                    break;
                case "rangedMax":
                {
                    string max = root.GetAttributeValue<string>("max");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, max, false);
                }
                    break;
                case "rangedMin":
                {
                    string min = root.GetAttributeValue<string>("min");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, min, true);
                }
                    break;
                case "fixedList":
                    optionBehaviour = CreateFixedListBehaviour(root, optionValue);
                    break;
                case "flagList":
                    optionBehaviour = CreateFlagListBehaviour(root, optionValue);
                    break;
                case "multiList":
                {
                    bool sorted = root.GetAttributeValue<bool>("sorted");
                    string separator = root.GetAttributeValue<string>("separator");
                    optionBehaviour = CreateMultiListBehaviour(root, optionValue, sorted, separator);
                }
                    break;
                case "sqlFixedList":
                {
                    string query = root.GetAttributeValue<string>("query");
                    string valueFieldName = root.GetAttributeValue<string>("valueFieldName");
                    string displayValueFieldName = root.GetAttributeValue<string>("displayValueFieldName");
                    optionBehaviour = new SqlFixedListOptionBehaviour(optionValue, query, valueFieldName, displayValueFieldName);
                }
                    break;
                case "sqlFlagList":
                {
                    string query = root.GetAttributeValue<string>("query");
                    string valueFieldName = root.GetAttributeValue<string>("valueFieldName");
                    string displayValueFieldName = root.GetAttributeValue<string>("displayValueFieldName");
                    optionBehaviour = new SqlFlagListOptionBehaviour(optionValue, query, valueFieldName, displayValueFieldName);
                }
                    break;
                case "sqlMultiList":
                {
                    string query = root.GetAttributeValue<string>("query");
                    bool sorted = root.GetAttributeValue<bool>("sorted");
                    string separator = root.GetAttributeValue<string>("separator");
                    string valueFieldName = root.GetAttributeValue<string>("valueFieldName");
                    string displayValueFieldName = root.GetAttributeValue<string>("displayValueFieldName");
                    optionBehaviour = new SqlMultiListOptionBehaviour(optionValue, query, sorted, separator, valueFieldName, displayValueFieldName);
                }
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
                    var value = optionValue.GetValue(element.GetAttributeValue<string>("value"));
                    string displayValue = element.GetAttributeValue<string>("displayValue");
                    list.Add(new ListItem(value, displayValue));
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
