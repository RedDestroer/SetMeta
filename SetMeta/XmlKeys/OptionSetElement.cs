namespace SetMeta.XmlKeys
{
    // ReSharper disable MemberHidesStaticFromOuterClass
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

                    internal static class Defaults
                    {
                        public const string ValueFieldName = "value";
                        public const string DisplayValueFieldName = "displayValue";
                    }
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

                    internal static class Defaults
                    {
                        public const string ValueFieldName = "value";
                        public const string DisplayValueFieldName = "displayValue";
                    }
                }
            }

            internal static class MultiListElement
            {
                public const string ElementName = "multiList";

                internal static class Attrs
                {
                    public const string Sorted = "sorted";
                    public const string Separator = "separator";

                    internal static class Defaults
                    {
                        public const bool Sorted = false;
                        public const string Separator = ";";
                    }
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

                    internal static class Defaults
                    {
                        public const bool Sorted = false;
                        public const string Separator = ";";
                        public const string ValueFieldName = "value";
                        public const string DisplayValueFieldName = "displayValue";
                    }
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

                internal static class Defaults
                {
                    public const string DisplayName = null;
                    public const string Description = null;
                }
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

                internal static class Defaults
                {
                    public const string Value = null;
                }
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