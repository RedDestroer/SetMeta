using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class Group
    {
        public Group()
        {
            Options = new Dictionary<string, Option>();
            Groups = new Dictionary<string, Group>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public IDictionary<string, Option> Options { get; }
        public IDictionary<string, Group> Groups { get; }
    }

    internal static class GroupAttributeKeys
    {
        public const string Name = "name";
        public const string DisplayName = "displayName";
        public const string Description = "description";
    }

    internal static class GroupAttributeDefaults
    {
        public const string DisplayName = null;
        public const string Description = null;
    }
}