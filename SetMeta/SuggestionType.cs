﻿namespace SetMeta
{
    /// <summary>
    /// Тип предложения. Нескольких предложений с одним типом для одной группы настроек или настройки быть не может
    /// </summary>
    public enum SuggestionType
        : byte
    {
        /// <summary>
        /// Желаемая максимальная длина поля ввода
        /// </summary>
        MaxLength = 0,

        /// <summary>
        /// Поле ввода должно быть многострочным
        /// </summary>
        Multiline = 1,

        /// <summary>
        /// Минимальное количество строк многострочного поля
        /// </summary>
        MinLines = 2,

        /// <summary>
        /// Максимальное количество строк многострочного поля
        /// </summary>
        MaxLines = 3,

        /// <summary>
        /// Регулярное выражение для проверки значения поля
        /// </summary>
        Regex = 4,

        /// <summary>
        /// Уведомлять ли друге поля об изменении этого поля
        /// </summary>
        NotifyOnChange = 5,

        /// <summary>
        /// Получать ли уведомления от других полей об их изменении
        /// </summary>
        Notifiable = 6,

        /// <summary>
        /// Желаемая минимальная длина поля ввода
        /// </summary>
        MinLength = 7,
    }
}