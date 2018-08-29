namespace SetMeta.Entities.Suggestions
{
    public class NotifiableSuggestion
        :Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.Notifiable;
    }
}