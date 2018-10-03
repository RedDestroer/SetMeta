namespace SetMeta.Entities.Suggestions
{
    public class MinLinesSuggestion
        : Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.MinLines;

        /// <summary>
        /// Желаемое минимальное количество строк
        /// </summary>
        public byte Value { get; set; }
    }
}