using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class Group
    {
        public Group()
        {
            GroupOptions = new List<GroupOption>();
            Groups = new Dictionary<string, Group>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public IList<GroupOption> GroupOptions { get; }
        public IDictionary<string, Group> Groups { get; }
    }
}