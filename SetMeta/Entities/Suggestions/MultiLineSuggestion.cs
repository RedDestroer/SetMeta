namespace SetMeta.Entities.Suggestions
{
    public class MultiLineSuggestion
        : Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.Multiline;
    }
}