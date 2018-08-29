namespace SetMeta.Entities
{
    /// <summary>
    /// Предложение по группе настроек или настройке, которые хотелось бы чтобы учёл генератор
    /// </summary>
    public abstract class Suggestion
    {
        /// <summary>
        /// Тип предложения
        /// </summary>
        public abstract SuggestionType SuggestionType { get; }
    }
}