using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using SetMeta.Abstract;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Entities.Suggestions;
using SetMeta.Util;
using Group = SetMeta.Entities.Group;

namespace SetMeta.Impl
{
    internal class OptionSetParserV1
        : OptionSetParser
    {
        private readonly IOptionValueFactory _optionValueFactory;

        private IOptionSetValidator _optionSetValidator;
        private IDictionary<string, Constant> _constants;

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
            _constants = optionSet.Constants;

            ParseOptions(body, optionSet.Options);
            ParseGroups(body, optionSet.Groups);

            return optionSet;
        }

        private string CheckName(XElement root, string name)
        {
            var value = TryGetMandatoryAttributeValue<string>(root, name);

            Regex regex = new Regex(@"(^\w+|_+)");
            MatchCollection matches = regex.Matches(value);

            if (matches.Count > 0)
            {
                Regex regexName = new Regex(@"(^\d+)");
                MatchCollection matchesName = regexName.Matches(value);

                if (matchesName.Count > 0)
                {
                    _optionSetValidator.AddError($"Name {value} isn`t valid.", root);
                }              
            }
            else
            {
                _optionSetValidator.AddError($"Name {value} isn`t valid.", root);
            }

            return value;
        }

        private T ReplaceConstants<T>(XElement root, string attributeName)
        {
            var value = root.GetAttributeValue<string>(attributeName);
            value = ReplaceConstants(value);

            return DataConversion.Convert<T>(value);
        }

        private string ReplaceConstants(string value)
        {
            if (value != null)
            {
                Regex regex = new Regex(@"{(Constant name=)(?<name>\w*|_*)}");
                MatchCollection matches = regex.Matches(value);

                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string name = match.Groups["name"].Value;

                        foreach (var constant in _constants)
                        {
                            if (string.Equals(constant.Value.Name, name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                value = regex.Replace(value, Convert.ToString(constant.Value.Value));
                            }
                        }
                    }
                }
            }

            return value;
        }

        private void ParseConstants(XElement root, IDictionary<string, Constant> constants)
        {
            root.Elements(Keys.Constant)
                .ForEach(element =>
                {
                    var constant = ParseConstant(element);
                    if (KeyIsValid(constant.Name.ToLower()))
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

            group.Name = CheckName(root, GroupAttributeKeys.Name);
            group.Id = CreateId(group.Name);
            group.DisplayName = ReplaceConstants(root.TryGetAttributeValue<string>(GroupAttributeKeys.DisplayName, GroupAttributeDefaults.DisplayName));
            group.Description = ReplaceConstants(root.TryGetAttributeValue<string>(GroupAttributeKeys.Description, GroupAttributeDefaults.Description));
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
                    ushort max = ReplaceConstants<ushort>(root, "value");
                    suggestion = new MaxLengthSuggestion {Value = max};
                }
                    break;
                case "maxLines":
                {
                    byte max = ReplaceConstants<byte>(root, "value");
                    suggestion = new MaxLinesSuggestion {Value = max};
                }
                    break;
                case "minLength":
                {
                    ushort min = ReplaceConstants<ushort>(root, "value");
                    suggestion = new MinLengthSuggestion { Value = min };
                }
                    break;
                case "minLines":
                {
                    byte min = ReplaceConstants<byte>(root, "value");
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
                    string value = ReplaceConstants<string>(root, "value");
                    string validation = ReplaceConstants(root.TryGetAttributeValue<string>("validation", null));
                    suggestion = new RegexSuggestion { Value = value, Validation = validation};
                }
                    break;
            }

            return suggestion != null;
        }
        
        private Constant ParseConstant(XElement root)
        {
            var constant = new Constant();

            constant.Name = CheckName(root, ConstantAttributeKeys.Name);
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

            option.Name = CheckName(root, OptionAttributeKeys.Name);
            option.Id = CreateId(option.Name);
            option.DisplayName = ReplaceConstants(root.TryGetAttributeValue<string>(OptionAttributeKeys.DisplayName, OptionAttributeDefaults.DisplayName));
            option.Description = ReplaceConstants(root.TryGetAttributeValue<string>(OptionAttributeKeys.Description, OptionAttributeDefaults.Description));
            option.DefaultValue = ReplaceConstants(root.TryGetAttributeValue<string>(OptionAttributeKeys.DefaultValue, null));
            option.ValueType = root.TryGetAttributeValue(OptionAttributeKeys.ValueType, OptionAttributeDefaults.ValueType);
            var optionValue = _optionValueFactory.Create(option.ValueType);
            option.Behaviour = CreateBehaviour(root, optionValue);

            var defaultValueElement = root.Elements().FirstOrDefault(o => o.Name == OptionAttributeKeys.DefaultValue);
            if (defaultValueElement != null)
            {
                if (root.IsAttributeExists(OptionAttributeKeys.DefaultValue))
                    _optionSetValidator.AddError($"Option {option.Name} has two defaultValue's.", root);

                var dataElement = defaultValueElement.Elements().First();
                if (dataElement.NodeType != XmlNodeType.CDATA)
                    _optionSetValidator.AddError("DefaultValue element doesn't contains CData.", dataElement);

                option.DefaultValue = dataElement.Value.Trim();
            }

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
                    string min = ReplaceConstants<string>(root, "min");
                    string max = ReplaceConstants<string>(root, "max");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, min, max);
                }
                    break;
                case "rangedMax":
                {
                    string max = ReplaceConstants<string>(root, "max");
                    optionBehaviour = new RangedOptionBehaviour(optionValue, max, false);
                }
                    break;
                case "rangedMin":
                {
                    string min = ReplaceConstants<string>(root, "min");
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
                    bool sorted = ReplaceConstants<bool>(root, "sorted");
                    string separator = ReplaceConstants<string>(root, "separator");
                    optionBehaviour = CreateMultiListBehaviour(root, optionValue, sorted, separator);
                }
                    break;
                case "sqlFixedList":
                {
                    string query = ReplaceConstants<string>(root, "query");
                    string valueFieldName = ReplaceConstants(root.TryGetAttributeValue<string>("valueFieldName", "value"));
                    string displayValueFieldName = ReplaceConstants(root.TryGetAttributeValue<string>("displayValueFieldName", "displayValue"));
                    optionBehaviour = new SqlFixedListOptionBehaviour(optionValue, query, valueFieldName, displayValueFieldName);
                }
                    break;
                case "sqlFlagList":
                {
                    string query = ReplaceConstants<string>(root, "query");
                    string valueFieldName = ReplaceConstants(root.TryGetAttributeValue<string>("valueFieldName", "value"));
                    string displayValueFieldName = ReplaceConstants(root.TryGetAttributeValue<string>("displayValueFieldName", "displayValue"));
                    optionBehaviour = new SqlFlagListOptionBehaviour(optionValue, query, valueFieldName, displayValueFieldName);
                }
                    break;
                case "sqlMultiList":
                {
                    string query = ReplaceConstants<string>(root, "query");
                    bool sorted = DataConversion.Convert<bool>(ReplaceConstants(root.TryGetAttributeValue<bool>("sorted", false).ToString()));
                    string separator = ReplaceConstants(root.TryGetAttributeValue<string>("separator", ";"));
                    string valueFieldName = ReplaceConstants(root.TryGetAttributeValue<string>("valueFieldName", "value"));
                    string displayValueFieldName = ReplaceConstants(root.TryGetAttributeValue<string>("displayValueFieldName", "displayValue"));
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
