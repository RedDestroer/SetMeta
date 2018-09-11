namespace SetMeta.Entities.Suggestions
{
    public class NotifyOnChangeSuggestion
        : Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.NotifyOnChange;
    }
}