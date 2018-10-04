namespace SetMeta.Entities.Suggestions
{
    public class MaxLinesSuggestion
        : Suggestion
    {
        /// <summary>
        /// Suggestion type
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.MaxLines;

        /// <summary>
        /// Desired maximum number of lines 
        /// </summary>
        public byte Value { get; set; }
    }
}