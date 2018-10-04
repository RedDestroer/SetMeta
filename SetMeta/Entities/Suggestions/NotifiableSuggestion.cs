namespace SetMeta.Entities.Suggestions
{
    public class NotifiableSuggestion
        : Suggestion
    {
        /// <summary>
        /// Suggestion type
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.Notifiable;
    }
}