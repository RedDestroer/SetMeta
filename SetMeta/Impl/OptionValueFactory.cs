using System;
using System.ComponentModel;
using SetMeta.Abstract;

namespace SetMeta.Impl
{
    public class OptionValueFactory
        : IOptionValueFactory
    {
        public IOptionValue Create(OptionValueType optionValueType)
        {
            switch (optionValueType)
            {
                case OptionValueType.String:
                    return new OptionValue<string>(optionValueType, new OptionValueConverter<string>());               
                case OptionValueType.Byte:
                    return new OptionValue<byte>(optionValueType, new OptionValueConverter<byte>());
                case OptionValueType.Int16:
                    return new OptionValue<Int16>(optionValueType, new OptionValueConverter<Int16>());
                case OptionValueType.UInt16:
                    return new OptionValue<UInt16>(optionValueType, new OptionValueConverter<UInt16>());
                case OptionValueType.Int32:
                    return new OptionValue<Int32>(optionValueType, new OptionValueConverter<Int32>());
                case OptionValueType.UInt32:
                    return new OptionValue<UInt32>(optionValueType, new OptionValueConverter<UInt32>());
                case OptionValueType.Int64:
                    return new OptionValue<Int64>(optionValueType, new OptionValueConverter<Int64>());
                case OptionValueType.UInt64:
                    return new OptionValue<UInt64>(optionValueType, new OptionValueConverter<UInt64>());
                case OptionValueType.Guid:
                    return new OptionValue<Guid>(optionValueType, new OptionValueConverter<Guid>());
                case OptionValueType.Bool:
                    return new OptionValue<bool>(optionValueType, new OptionValueConverter<bool>());
                case OptionValueType.Decimal:
                    return new OptionValue<decimal>(optionValueType, new OptionValueConverter<decimal>());
                case OptionValueType.DateTime:
                    return new OptionValue<DateTime>(optionValueType, new OptionValueConverter<DateTime>());
                case OptionValueType.Date:
                    return new OptionValue<DateTime>(optionValueType, new OptionValueConverter<DateTime>());
                case OptionValueType.Time:
                    return new OptionValue<DateTime>(optionValueType, new OptionValueConverter<DateTime>());
                case OptionValueType.TimeSpan:
                    return new OptionValue<TimeSpan>(optionValueType, new OptionValueConverter<TimeSpan>());
                case OptionValueType.NullableByte:
                    return new OptionValue<byte?>(optionValueType, new OptionValueConverter<byte?>());
                case OptionValueType.NullableInt16:
                    return new OptionValue<Int16?>(optionValueType, new OptionValueConverter<Int16?>());
                case OptionValueType.NullableUInt16:
                    return new OptionValue<UInt16?>(optionValueType, new OptionValueConverter<UInt16?>());
                case OptionValueType.NullableInt32:
                    return new OptionValue<Int32?>(optionValueType, new OptionValueConverter<Int32?>());
                case OptionValueType.NullableUInt32:
                    return new OptionValue<UInt32?>(optionValueType, new OptionValueConverter<UInt32?>());
                case OptionValueType.NullableInt64:
                    return new OptionValue<Int64?>(optionValueType, new OptionValueConverter<Int64?>());
                case OptionValueType.NullableUInt64:
                    return new OptionValue<UInt64?>(optionValueType, new OptionValueConverter<UInt64?>());
                case OptionValueType.NullableGuid:
                    return new OptionValue<Guid?>(optionValueType, new OptionValueConverter<Guid?>());
                case OptionValueType.NullableBool:
                    return new OptionValue<bool?>(optionValueType, new OptionValueConverter<bool?>());
                case OptionValueType.NullableDecimal:
                    return new OptionValue<decimal?>(optionValueType, new OptionValueConverter<decimal?>());
                case OptionValueType.NullableDateTime:
                    return new OptionValue<DateTime?>(optionValueType, new OptionValueConverter<DateTime?>());
                case OptionValueType.NullableDate:
                    return new OptionValue<DateTime?>(optionValueType, new OptionValueConverter<DateTime?>());
                case OptionValueType.NullableTime:
                    return new OptionValue<DateTime?>(optionValueType, new OptionValueConverter<DateTime?>());
                case OptionValueType.NullableTimeSpan:
                    return new OptionValue<TimeSpan?>(optionValueType, new OptionValueConverter<TimeSpan?>());
                case OptionValueType.ByteArray:
                    return new OptionValue<Byte[]>(optionValueType, new OptionValueConverter<Byte[]>());
                default:
                    throw new InvalidEnumArgumentException(nameof(optionValueType), (int)optionValueType, typeof(OptionValueType));
            }
        }
    }
}