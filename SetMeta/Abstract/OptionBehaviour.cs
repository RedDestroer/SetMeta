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
        /// Setting value type
        /// </summary>
        public OptionValueType OptionValueType => OptionValue.OptionValueType;

        /// <summary>
        /// Object with setting value
        /// </summary>
        protected IOptionValue OptionValue { get; }

        /// <summary>
        /// Gets setting value from string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object Value(string value)
        {
            return OptionValue.GetValue(value);
        }

        /// <summary>
        /// Gets string from setting value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string StringValue(object value)
        {
            return OptionValue.GetStringValue(value);
        }
    }
}
