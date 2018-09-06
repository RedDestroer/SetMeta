using System;
using SetMeta.Abstract;

namespace SetMeta.Entities
{
    public class Option
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public OptionValueType ValueType { get; set; }
        public OptionBehaviour Behaviour { get; set; }
    }

    [Obsolete]
    internal static class Keys
    {
        public const string OptionSet = "optionSet";
        public const string Option = "option";
        public const string Group = "group";
        public const string Constant = "constant";
        public const string Suggestion = "suggestion";
    }

    [Obsolete]
    internal static class OptionSetAttributeKeys
    {
        public const string Version = "version";
    }

    [Obsolete]
    internal static class OptionAttributeKeys
    {
        public const string Name = "name";
        public const string DisplayName = "displayName";
        public const string Description = "description";
        public const string DefaultValue = "defaultValue";
        public const string ValueType = "valueType";
    }

    [Obsolete]
    internal static class OptionAttributeDefaults
    {
        public const string DisplayName = null;
        public const string Description = null;
        public const object DefaultValue = null;
        public const OptionValueType ValueType = OptionValueType.String;
    }

    internal static class OptionSetElement
    {
        public const string ElementName = "optionSet";

        internal static class Attrs
        {
            public const string Version = "version";
        }

        internal static class OptionElement
        {
            public const string ElementName = "option";

            internal static class Attrs
            {
                public const string Name = "name";
                public const string DisplayName = "displayName";
                public const string Description = "description";
                public const string DefaultValue = "defaultValue";
                public const string ValueType = "valueType";

                internal static class Defaults
                {
                    public const string DisplayName = null;
                    public const string Description = null;
                    public const object DefaultValue = null;
                    public const OptionValueType ValueType = OptionValueType.String;
                }
            }

            internal static class DefaultValue
            {
                public const string ElementName = "defaultValue";
            }

            internal static class RangedMinMaxElement
            {
                public const string ElementName = "rangedMinMax";

                internal static class Attrs
                {
                    public const string Min = "min";
                    public const string Max = "max";
                }
            }

            internal static class RangedMaxElement
            {
                public const string ElementName = "rangedMax";

                internal static class Attrs
                {
                    public const string Max = "max";
                }
            }

            internal static class RangedMinElement
            {
                public const string ElementName = "rangedMin";

                internal static class Attrs
                {
                    public const string Min = "min";
                }
            }

            internal static class FixedListElement
            {
                public const string ElementName = "fixedList";

                internal static class ListItemElement
                {
                    public const string ElementName = "listItem";

                    internal static class Attrs
                    {
                        public const string Value = "value";
                        public const string DisplayValue = "displayValue";
                    }
                }
            }

            internal static class SqlFixedListElement
            {
                public const string ElementName = "sqlFixedList";

                internal static class Attrs
                {
                    public const string Query = "query";
                    public const string ValueFieldName = "valueFieldName";
                    public const string DisplayValueFieldName = "displayValueFieldName";
                }
            }

            internal static class FlagListElement
            {
                public const string ElementName = "flagList";

                internal static class ListItemElement
                {
                    public const string ElementName = "listItem";

                    internal static class Attrs
                    {
                        public const string Value = "value";
                        public const string DisplayValue = "displayValue";
                    }
                }
            }

            internal static class SqlFlagListElement
            {
                public const string ElementName = "sqlFlagList";

                internal static class Attrs
                {
                    public const string Query = "query";
                    public const string ValueFieldName = "valueFieldName";
                    public const string DisplayValueFieldName = "displayValueFieldName";
                }
            }

            internal static class MultiListElement
            {
                public const string ElementName = "multiList";

                internal static class Attrs
                {
                    public const string Sorted = "sorted";
                    public const string Separator = "separator";
                }

                internal static class ListItemElement
                {
                    public const string ElementName = "listItem";

                    internal static class Attrs
                    {
                        public const string Value = "value";
                        public const string DisplayValue = "displayValue";
                    }
                }
            }

            internal static class SqlMultiListElement
            {
                public const string ElementName = "sqlMultiList";

                internal static class Attrs
                {
                    public const string Sorted = "sorted";
                    public const string Separator = "separator";
                    public const string Query = "query";
                    public const string ValueFieldName = "valueFieldName";
                    public const string DisplayValueFieldName = "displayValueFieldName";
                }
            }
        }

        internal static class GroupElement
        {
            public const string ElementName = "group";

            internal static class Attrs
            {
                // ReSharper disable once MemberHidesStaticFromOuterClass
                public const string Name = "name";
                public const string DisplayName = "displayName";
                public const string Description = "description";
            }

            internal static class OptionElement
            {
                public const string ElementName = "option";

                internal static class Attrs
                {
                    // ReSharper disable once MemberHidesStaticFromOuterClass
                    public const string Name = "name";
                }

                internal static class SuggestionElement
                {
                    public const string ElementName = "suggestion";

                    internal static class Attrs
                    {
                        // ReSharper disable once MemberHidesStaticFromOuterClass
                        public const string Name = "name";
                    }
                }
            }
        }

        internal static class ConstantElement
        {
            public const string ElementName = "constant";

            internal static class Attrs
            {
                // ReSharper disable once MemberHidesStaticFromOuterClass
                public const string Name = "name";
                public const string Value = "value";
                public const string ValueType = "valueType";
            }
        }

        internal static class SuggestionElement
        {
            public const string ElementName = "suggestion";

            internal static class Attrs
            {
                // ReSharper disable once MemberHidesStaticFromOuterClass
                public const string Name = "name";
            }

            internal static class MinLengthElement
            {
                public const string ElementName = "minLength";

                internal static class Attrs
                {
                    public const string Value = "value";
                }
            }

            internal static class MaxLengthElement
            {
                public const string ElementName = "maxLength";

                internal static class Attrs
                {
                    public const string Value = "value";
                }
            }

            internal static class MultilineElement
            {
                public const string ElementName = "multiline";
            }

            internal static class MinLinesElement
            {
                public const string ElementName = "minLines";

                internal static class Attrs
                {
                    public const string Value = "value";
                }
            }

            internal static class MaxLinesElement
            {
                public const string ElementName = "maxLines";

                internal static class Attrs
                {
                    public const string Value = "value";
                }
            }

            internal static class RegexElement
            {
                public const string ElementName = "regex";

                internal static class Attrs
                {
                    public const string Value = "value";
                    public const string Validation = "validation";
                }
            }

            internal static class NotifyOnChangeElement
            {
                public const string ElementName = "notifyOnChange";
            }

            internal static class NotifiableElement
            {
                public const string ElementName = "notifiable";
            }

            internal static class ControlElement
            {
                public const string ElementName = "control";

                internal static class Attrs
                {
                    public const string Value = "value";
                }
            }
        }
    }    
}
