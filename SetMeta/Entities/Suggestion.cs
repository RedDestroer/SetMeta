namespace SetMeta.Entities
{
    /// <summary>
    /// Proposal for a group of settings or setting that would be great to take into account the generator
    /// </summary>
    public abstract class Suggestion
    {
        /// <summary>
        /// Suggestion type
        /// </summary>
        public abstract SuggestionType SuggestionType { get; }
    }
}