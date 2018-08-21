using System;
using SetMeta.Abstract;
using SetMeta.Util;

namespace SetMeta.Impl
{
    internal class OptionValue<T>
        : IOptionValue<T>
    {
        private readonly IOptionValueConverter<T> _optionValueConverter;

        internal OptionValue(OptionValueType optionValueType, IOptionValueConverter<T> optionValueConverter)
        {
            OptionValueType = optionValueType;
            _optionValueConverter = optionValueConverter;
            ValueType = typeof(T);
        }

        public OptionValueType OptionValueType { get; }

        public Type ValueType { get; }

        public T GetValue(string value)
        {
            return DataConversion.Convert<T>(value);
        }

        public string GetStringValue(T value)
        {
            return _optionValueConverter.GetStringValue(value);
        }

        public string GetStringValue(T value, IFormatProvider formatProvider)
        {
            return _optionValueConverter.GetStringValue(value, formatProvider);
        }

        public string GetStringValue(object value)
        {
            return GetStringValue((T)value);
        }

        public string GetStringValue(object value, IFormatProvider formatProvider)
        {
            return GetStringValue((T)value, formatProvider);
        }

        object IOptionValue.GetValue(string value)
        {
            return GetValue(value);
        }
    }
}