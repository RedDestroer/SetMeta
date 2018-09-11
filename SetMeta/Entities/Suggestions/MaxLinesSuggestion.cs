namespace SetMeta.Entities.Suggestions
{
    public class MaxLinesSuggestion
        : Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.MaxLines;

        /// <summary>
        /// Желаемое максимальное количество строк
        /// </summary>
        public byte Value { get; set; }
    }
}