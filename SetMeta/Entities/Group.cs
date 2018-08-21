using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class Group
    {
        public Group()
        {
            Options = new Dictionary<string, Option>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public IDictionary<string, Option> Options { get; }
    }
}