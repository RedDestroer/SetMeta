using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class OptionSet
    {
        public OptionSet()
        {
Options = new List<Option>();
        }

        public string Version { get; set; }
 public IList<Option> Options { get; }
    }
}