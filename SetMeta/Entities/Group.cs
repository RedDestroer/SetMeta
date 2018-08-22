using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class Group
    {
        public Group()
        {
            Groups = new List<Group>();
            Options = new List<Option>();
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public IList<Group> Groups { get; }
        public IList<Option> Options { get; }
    }
}