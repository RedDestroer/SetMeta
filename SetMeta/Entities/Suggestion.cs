using System.Collections.Generic;

namespace SetMeta.Entities
{
    /// <summary>
    /// Proposal for a setting that would be great to take into account the generator
    /// </summary>
    public class Suggestion
    {
        public Suggestion()
        {
            Params = new Dictionary<SuggestionType, IDictionary<string, string>>();
        }

        // You have to remove this.
        /////// <summary>
        /////// Suggestion type
        /////// </summary>
        ////public abstract SuggestionType SuggestionType { get; }
        
        public string Id { get; set; }
        public string Name { get; set; }
        public IDictionary<SuggestionType, IDictionary<string, string>> Params { get; }
    }
}