namespace SetMeta.Entities.Suggestions
{
    public class NotifyOnChangeSuggestion
        : Suggestion
    {
        /// <summary>
        /// Suggestion type
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.NotifyOnChange;
    }
}