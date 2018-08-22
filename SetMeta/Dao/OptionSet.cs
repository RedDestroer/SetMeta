using System.Collections.Generic;

namespace SetMeta.Dao
{
    public class OptionSet
    {
        public OptionSet()
        {
            Options = new List<Option>();
            Groups = new List<Group>();
        }

        public string Version { get; set; }
        public IList<Option> Options { get; }
        public IList<Group> Groups { get; }
    }

    public class Option
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public OptionValueType ValueType { get; set; }
        public OptionBehavior Behavior { get; set; }
    }

    public abstract class OptionBehavior
    {
    }

    public class SimpleOptionBehaviour
        : OptionBehavior
    {
    }

    public class RangedOptionBehaviour
        : OptionBehavior
    {
        public string MinValue { get; set; }
        public bool IsMinValueExists { get; set; }
        public string MaxValue { get; set; }
        public bool IsMaxValueExists { get; set; }
    }

    public class FixedListOptionBehaviour
        : OptionBehavior
    {
        public FixedListOptionBehaviour()
        {
            ListItems = new List<KeyValuePair<string, string>>();
        }

        public IList<KeyValuePair<string, string>> ListItems { get; }
    }

    public class SqlFixedListOptionBehaviour
        : OptionBehavior
    {
        public string Query { get; }
        public string ValueMember { get; }
        public string DisplayMember { get; }
    }

    public class FlagListOptionBehaviour
        : OptionBehavior
    {
        public FlagListOptionBehaviour()
        {
            ListItems = new List<KeyValuePair<string, string>>();
        }

        public IList<KeyValuePair<string, string>> ListItems { get; }
    }

    public class SqlFlagListOptionBehaviour
        : OptionBehavior
    {
        public string Query { get; set; }
        public string ValueMember { get; set; }
        public string DisplayMember { get; set; }
    }

    public class MultiListOptionBehaviour
        : OptionBehavior
    {
        public MultiListOptionBehaviour()
        {
            ListItems = new List<KeyValuePair<string, string>>();
        }

        public IList<KeyValuePair<string, string>> ListItems { get; }
        public bool Sorted { get; set; }
        public string Separator { get; set; }
    }

    public class SqlMultiListOptionBehaviour
        : OptionBehavior
    {
        public bool Sorted { get; set; }
        public string Separator { get; set; }
        public string Query { get; set; }
        public string ValueMember { get; set; }
        public string DisplayMember { get; set; }
    }

    public class Group
    {
        public Group()
        {
            Groups = new List<Group>();
            Options = new List<string>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public IList<Group> Groups { get; }
        public IList<string> Options { get; }
    }

    internal static class Keys
    {
        public const string OptionSet = "optionSet";
        public const string Option = "option";
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
        public const string DefaultValue = null;
        public const OptionValueType ValueType = OptionValueType.String;
    }
}