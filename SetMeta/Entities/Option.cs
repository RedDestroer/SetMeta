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
}
