namespace SetMeta
{
    /// <summary>
    /// Suggestion type. There can't be several suggestion with one type for one setting group or setting
    /// </summary>
    public enum SuggestionType
        : byte
    {
        /// <summary>
        /// Desired maximum input field length
        /// </summary>
        MaxLength = 0,

        /// <summary>
        /// Input field must be multi-lined
        /// </summary>
        Multiline = 1,

        /// <summary>
        /// Minimum number of lines of multi-line field
        /// </summary>
        MinLines = 2,

        /// <summary>
        /// Maximum number of lines of multi-line field
        /// </summary>
        MaxLines = 3,

        /// <summary>
        /// Regular expression to check the field value
        /// </summary>
        Regex = 4,

        /// <summary>
        /// Whether to notify fields about changes to this field.
        /// </summary>
        NotifyOnChange = 5,

        /// <summary>
        /// Whether to receive notifications from other fields about changes
        /// </summary>
        Notifiable = 6,

        /// <summary>
        /// Desired minimum input field length
        /// </summary>
        MinLength = 7,
    }
}