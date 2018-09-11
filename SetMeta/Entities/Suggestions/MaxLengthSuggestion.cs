namespace SetMeta.Entities.Suggestions
{
    public class MaxLengthSuggestion
        : Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.MaxLength;

        /// <summary>
        /// Желаемая максимальная длина поля
        /// </summary>
        public ushort Value { get; set; }
    }
}