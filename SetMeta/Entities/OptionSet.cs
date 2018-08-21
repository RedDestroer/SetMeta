using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class OptionSet
    {
        public OptionSet()
        {
            Groups = new Dictionary<string, Group>();
            Options = new Dictionary<string, Option>();
        }

        public string Version { get; set; }
        public IDictionary<string, Group> Groups { get; }
        public IDictionary<string, Option> Options { get; }
    }
}