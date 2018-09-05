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

    internal static class Keys
    {
        public const string OptionSet = "optionSet";
        public const string Option = "option";
        public const string Group = "group";
        public const string Constant = "constant";
        public const string Suggestion = "suggestion";
    }

    internal static class OptionSetAttributeKeys
    {
        public const string Version = "version";
    }

    internal static class OptionAttributeKeys
    {
        public const string Name = "name";
        public const string DisplayName = "displayName";
        public const string Description = "description";
        public const string DefaultValue = "defaultValue";
        public const string ValueType = "valueType";
    }

    internal static class OptionAttributeDefaults
    {
        public const string DisplayName = null;
        public const string Description = null;
        public const object DefaultValue = null;
        public const OptionValueType ValueType = OptionValueType.String;
    }

    internal static class RangedMinMaxBehaviourKeys
    {
        public const string Name = "rangedMinMax";

        internal static class AttrKeys
        {
            public const string Min = "min";
            public const string Max = "max";
        }
    }

    internal static class RangedMaxBehaviourKeys
    {
        public const string Name = "rangedMax";

        internal static class AttrKeys
        {
            public const string Max = "max";
        }
    }

    internal static class RangedMinBehaviourKeys
    {
        public const string Name = "rangedMin";

        internal static class AttrKeys
        {
            public const string Min = "min";
        }
    }

    internal static class FixedListBehaviourKeys
    {
        public const string Name = "fixedList";
    }

    internal static class ListItemKeys
    {
        public const string Name = "listItem";

        internal static class AttrKeys
        {
            public const string Value = "value";
            public const string DisplayValue = "displayValue";
        }
    }

    internal static class SqlFixedListBehaviourKeys
    {
        public const string Name = "sqlFixedList";

        internal static class AttrKeys
        {
            public const string Query = "query";
            public const string ValueFieldName = "valueFieldName";
            public const string DisplayValueFieldName = "displayValueFieldName";
        }
    }

    internal static class FlagListBehaviourKeys
    {
        public const string Name = "flagList";
    }

    internal static class SqlFlagListBehaviourKeys
    {
        public const string Name = "sqlFlagList";

        internal static class AttrKeys
        {
            public const string Query = "query";
            public const string ValueFieldName = "valueFieldName";
            public const string DisplayValueFieldName = "displayValueFieldName";
        }
    }

    internal static class MultiListBehaviourKeys
    {
        public const string Name = "multiList";

        internal static class AttrKeys
        {
            public const string Sorted = "sorted";
            public const string Separator = "separator";
        }
    }

    internal static class SqlMultiListBehaviourKeys
    {
        public const string Name = "sqlMultiList";

        internal static class AttrKeys
        {
            public const string Sorted = "sorted";
            public const string Separator = "separator";
            public const string Query = "query";
            public const string ValueFieldName = "valueFieldName";
            public const string DisplayValueFieldName = "displayValueFieldName";
        }
    }
}
