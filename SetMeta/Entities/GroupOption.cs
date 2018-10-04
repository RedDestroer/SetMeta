using System.Collections.Generic;

namespace SetMeta.Entities
{
    public class GroupOption
    {
        public GroupOption()
        {
            Suggestions = new Dictionary<SuggestionType, IDictionary<string, string>>();
        }

        public Option Option { get; set; }
        public IDictionary<SuggestionType, IDictionary<string, string>> Suggestions { get; }
    }
}