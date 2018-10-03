namespace SetMeta.Entities.Suggestions
{
    public class MinLengthSuggestion
        : Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.MinLength;

        /// <summary>
        /// Желаемая минимальная длина поля
        /// </summary>
        public ushort Value { get; set; }
    }
}