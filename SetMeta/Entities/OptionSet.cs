using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class OptionSet
    {
        public OptionSet()
        {
            Options = new Dictionary<string, Option>();
        }

        public string Version { get; set; }
        public IDictionary<string, Option> Options { get; }
    }
}