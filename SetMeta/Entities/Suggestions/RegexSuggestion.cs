namespace SetMeta.Entities.Suggestions
{
    public class RegexSuggestion
        :Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public override SuggestionType SuggestionType => SuggestionType.Regex;

        /// <summary>
        /// Регулярное выражение
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Валидационное сообщение, если значение не соответствует регулярному выражению
        /// </summary>
        public string Validation { get; set; }
    }
}