namespace SetMeta.Entities.Suggestions
{
    public class MaxLengthSuggestion
        : Suggestion
    {
        /// <summary>
        /// Suggestion type
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.MaxLength;

        /// <summary>
        /// Desired maximum field length
        /// </summary>
        public ushort Value { get; set; }
    }
}