namespace SetMeta.Entities.Suggestions
{
    public class MinLengthSuggestion
        : Suggestion
    {
        /// <summary>
        /// Suggestion type
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.MinLength;

        /// <summary>
        /// Desired minimum field length
        /// </summary>
        public ushort Value { get; set; }
    }
}