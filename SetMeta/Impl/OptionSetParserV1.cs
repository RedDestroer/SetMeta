﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using SetMeta.Abstract;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Util;
using Group = SetMeta.Entities.Group;
using OptionElement = SetMeta.XmlKeys.OptionSetElement.OptionElement;
using ConstantElement = SetMeta.XmlKeys.OptionSetElement.ConstantElement;
using GroupElement = SetMeta.XmlKeys.OptionSetElement.GroupElement;
using SuggestionElement = SetMeta.XmlKeys.OptionSetElement.SuggestionElement;

namespace SetMeta.Impl
{
    internal class OptionSetParserV1
        : OptionSetParser
    {
        private readonly IOptionValueFactory _optionValueFactory;

        private IOptionSetValidator _optionSetValidator;
        private IDictionary<string, Constant> _constants;
        private IDictionary<string, Suggestion> _suggestions;

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

            ParseSuggestions(body, optionSet.Suggestions);
            _suggestions = optionSet.Suggestions;

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
                    _optionSetValidator.AddError($"Name '{value}' isn`t valid.", root);
                }              
            }
            else
            {
                _optionSetValidator.AddError($"Name '{value}' isn`t valid.", root);
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
                var regex = new Regex(@"{(Constant name=)(?<name>\w*|_*)}");
                var matches = regex.Matches(value);

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
            root.Elements(ConstantElement.ElementName)
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
            root.Elements(GroupElement.ElementName)
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

            group.Name = CheckName(root, GroupElement.Attrs.Name);
            group.Id = CreateId(group.Name);
            group.DisplayName = ReplaceConstants(root.TryGetAttributeValue<string>(GroupElement.Attrs.DisplayName, GroupElement.Attrs.Defaults.DisplayName));
            group.Description = ReplaceConstants(root.TryGetAttributeValue<string>(GroupElement.Attrs.Description, GroupElement.Attrs.Defaults.Description));
            ParseGroupOptions(root, group.GroupOptions);
            ParseGroups(root, group.Groups);

            return group;
        }

        private void ParseGroupOptions(XElement root, IList<GroupOption> groupOptions)
        {
            root.Elements(GroupElement.OptionElement.ElementName)
                .ForEach(element =>
                {
                    var option = ParseOption(element);
                    if (KeyIsValid(option.Id))
                    {
                        if (KeyIsUniqueForGroupOption(option.Id, groupOptions))
                        {
                            var groupOption = new GroupOption();

                            groupOption.Option = option;
                            ParseSuggestions(element, groupOption.Suggestions);

                            groupOptions.AddIfUnique(groupOption);
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

        private bool KeyIsUniqueForGroupOption(string id, IList<GroupOption> groupOptions)
        {
            return groupOptions.All(groupOption => !groupOption.Option.Id.Equals(id));
        }

        private void ParseSuggestions(XElement root, IDictionary<string, Suggestion> suggestions)
        {
            root.Elements(SuggestionElement.ElementName)
                .ForEach(element =>
                {
                    var suggestionElements = CreateSuggestion(element);

                    if (suggestionElements.Count != 0)
                    {
                        var suggestion = new Suggestion();

                        suggestion.Name = CheckName(element, SuggestionElement.Attrs.Name);
                        suggestion.Id = CreateId(suggestion.Name);

                        foreach (var kv in suggestionElements)
                        {
                            suggestion.Params.Add(kv);
                        }

                        if (KeyIsUnique(suggestion.Name, suggestions))
                        {
                            suggestions[suggestion.Name] = suggestion;
                        }
                    }
                });
        }

        private void ParseSuggestions(XElement root, IDictionary<SuggestionType, IDictionary<string, string>> suggestions)
        {
            root.Elements(SuggestionElement.ElementName)
                .ForEach(element =>
                {
                    var suggestionName = CheckName(element, SuggestionElement.Attrs.Name);

                    foreach (var suggestion in _suggestions)
                    {
                        if (suggestion.Key.Equals(suggestionName))
                        {
                            foreach (var kv in suggestion.Value.Params)
                            {
                                suggestions[kv.Key] = kv.Value;
                            }
                        }
                    }
                });
        }

        private IDictionary<SuggestionType, IDictionary<string, string>> CreateSuggestion(XElement root)
        {
            var elements = root.Elements();
            var dictionary = new Dictionary<SuggestionType, IDictionary<string, string>>();

            foreach (var element in elements)
            {
                if (!TryCreateSuggestion(element, out var suggestion))
                {
                    if (!dictionary.ContainsKey(suggestion.Key))
                    {
                        dictionary.Add(suggestion.Key, suggestion.Value);
                    }
                }
            }

            return dictionary;
        }

        private bool TryCreateSuggestion(XElement root, out KeyValuePair<SuggestionType, IDictionary<string, string>> suggestion)
        {
            string name = root.Name.LocalName;
            

            switch (name)
            {
                case "maxLength":
                {
                    ushort max = ReplaceConstants<ushort>(root, "value");
                    var dictionary = new Dictionary<string, string> {{"value", max.ToString()}};
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.MaxLength, dictionary);
                }
                    break;
                case "maxLines":
                {
                    byte max = ReplaceConstants<byte>(root, "value");
                    var dictionary = new Dictionary<string, string> { { "value", max.ToString() } };
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.MaxLines, dictionary);
                }
                    break;
                case "minLength":
                {
                    ushort min = ReplaceConstants<ushort>(root, "value");
                    var dictionary = new Dictionary<string, string> { { "value", min.ToString() } };
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.MinLength, dictionary);
                }
                    break;
                case "minLines":
                {
                    byte min = ReplaceConstants<byte>(root, "value");
                    var dictionary = new Dictionary<string, string> { { "value", min.ToString() } };
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.MinLines, dictionary);
                }
                    break;
                case "multiline":
                {
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.Multiline, null);
                }
                    break;
                case "notifiable":
                {
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.Notifiable, null);
                }
                    break;
                case "notifyOnChange":
                {
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.NotifyOnChange, null);
                }
                    break;
                case "regex":
                {
                    string value = ReplaceConstants<string>(root, "value");
                    string validation = ReplaceConstants(root.TryGetAttributeValue<string>("validation", null));
                    var dictionary = new Dictionary<string, string> { { "value", value }, {"validation", validation} };
                    suggestion = new KeyValuePair<SuggestionType, IDictionary<string, string>>(SuggestionType.Regex, dictionary);
                }
                    break;
            }

            return false;
        }
        
        private Constant ParseConstant(XElement root)
        {
            var constant = new Constant();

            constant.Name = CheckName(root, ConstantElement.Attrs.Name);
            constant.Value = ReplaceConstants(root.TryGetAttributeValue(ConstantElement.Attrs.Value, ConstantElement.Attrs.Defaults.Value));

            return constant;
        }

        private void ParseOptions(XElement root, IDictionary<string, Option> options)
        {
            root.Elements(OptionElement.ElementName)
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

        private bool KeyIsUnique(string id, IDictionary<string, Suggestion> suggestions)
        {
            return !suggestions.ContainsKey(id);
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

            option.Name = CheckName(root, OptionElement.Attrs.Name);
            option.Id = CreateId(option.Name);
            option.DisplayName = ReplaceConstants(root.TryGetAttributeValue<string>(OptionElement.Attrs.DisplayName, OptionElement.Attrs.Defaults.DisplayName));
            option.Description = ReplaceConstants(root.TryGetAttributeValue<string>(OptionElement.Attrs.Description, OptionElement.Attrs.Defaults.Description));
            option.DefaultValue = ReplaceConstants(root.TryGetAttributeValue<string>(OptionElement.Attrs.DefaultValue, null));
            option.ValueType = root.TryGetAttributeValue(OptionElement.Attrs.ValueType, OptionElement.Attrs.Defaults.ValueType);
            var optionValue = _optionValueFactory.Create(option.ValueType);
            option.Behaviour = CreateBehaviour(root, optionValue);

            var defaultValueElement = root.Elements().FirstOrDefault(o => o.Name == OptionElement.Attrs.DefaultValue);
            if (defaultValueElement != null)
            {
                if (root.IsAttributeExists(OptionElement.Attrs.DefaultValue))
                    _optionSetValidator.AddError($"Option '{option.Name}' has two defaultValue's.", root);

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
                _optionSetValidator.AddError($"Mandatory attribute '{name}' not found.", root);

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
                    bool sorted = DataConversion.Convert<bool>(ReplaceConstants(root.TryGetAttributeValue<bool>("sorted", false).ToString()));
                    string separator = ReplaceConstants(root.TryGetAttributeValue<string>("separator", ";"));
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
