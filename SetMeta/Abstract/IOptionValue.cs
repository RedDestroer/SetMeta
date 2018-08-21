using System;

namespace SetMeta.Abstract
{
    public interface IOptionValue
    {
        OptionValueType OptionValueType { get; }
        Type ValueType { get; }
        object GetValue(string value);
        string GetStringValue(object value);
        string GetStringValue(object value, IFormatProvider formatProvider);
    }

    public interface IOptionValue<T>
        : IOptionValue
    {
        new T GetValue(string value);
        string GetStringValue(T value);
        string GetStringValue(T value, IFormatProvider formatProvider);
    }
}