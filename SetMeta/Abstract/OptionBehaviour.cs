using System;
using SetMeta.Util;

namespace SetMeta.Abstract
{
    public abstract class OptionBehaviour
    {
        protected OptionBehaviour(IOptionValue optionValue)
        {
            Validate.NotNull(optionValue, nameof(optionValue));

            OptionValue = optionValue;
        }

        /// <summary>
        /// Тип значения настройки
        /// </summary>
        public OptionValueType OptionValueType => OptionValue.OptionValueType;

        /// <summary>
        /// Объект со значением настройи
        /// </summary>
        protected IOptionValue OptionValue { get; }

        /// <summary>
        /// Получает значение настройки из строки
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Value(string value)
        {
            return OptionValue.GetValue(value);
        }

        /// <summary>
        /// Получает строку из значения настройки
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string StringValue(object value)
        {
            return OptionValue.GetStringValue(value);
        }
    }
}
