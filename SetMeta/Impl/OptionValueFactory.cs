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
                    return new OptionValue<short>(optionValueType, new OptionValueConverter<short>());
                case OptionValueType.UInt16:
                    return new OptionValue<ushort>(optionValueType, new OptionValueConverter<ushort>());
                case OptionValueType.Int32:
                    return new OptionValue<int>(optionValueType, new OptionValueConverter<int>());
                case OptionValueType.UInt32:
                    return new OptionValue<uint>(optionValueType, new OptionValueConverter<uint>());
                case OptionValueType.Int64:
                    return new OptionValue<long>(optionValueType, new OptionValueConverter<long>());
                case OptionValueType.UInt64:
                    return new OptionValue<ulong>(optionValueType, new OptionValueConverter<ulong>());
                case OptionValueType.Guid:
                    return new OptionValue<Guid>(optionValueType, new OptionValueConverter<Guid>());
                case OptionValueType.Bool:
                    return new OptionValue<bool>(optionValueType, new OptionValueConverter<bool>());
                case OptionValueType.Decimal:
                    return new OptionValue<decimal>(optionValueType, new OptionValueConverter<decimal>());
                case OptionValueType.DateTime:
                case OptionValueType.Date:
                case OptionValueType.Time:
                    return new OptionValue<DateTime>(optionValueType, new OptionValueConverter<DateTime>());
                case OptionValueType.TimeSpan:
                    return new OptionValue<TimeSpan>(optionValueType, new OptionValueConverter<TimeSpan>());
                case OptionValueType.NullableByte:
                    return new OptionValue<byte?>(optionValueType, new OptionValueConverter<byte?>());
                case OptionValueType.NullableInt16:
                    return new OptionValue<short?>(optionValueType, new OptionValueConverter<short?>());
                case OptionValueType.NullableUInt16:
                    return new OptionValue<ushort?>(optionValueType, new OptionValueConverter<ushort?>());
                case OptionValueType.NullableInt32:
                    return new OptionValue<int?>(optionValueType, new OptionValueConverter<int?>());
                case OptionValueType.NullableUInt32:
                    return new OptionValue<uint?>(optionValueType, new OptionValueConverter<uint?>());
                case OptionValueType.NullableInt64:
                    return new OptionValue<long?>(optionValueType, new OptionValueConverter<long?>());
                case OptionValueType.NullableUInt64:
                    return new OptionValue<ulong?>(optionValueType, new OptionValueConverter<ulong?>());
                case OptionValueType.NullableGuid:
                    return new OptionValue<Guid?>(optionValueType, new OptionValueConverter<Guid?>());
                case OptionValueType.NullableBool:
                    return new OptionValue<bool?>(optionValueType, new OptionValueConverter<bool?>());
                case OptionValueType.NullableDecimal:
                    return new OptionValue<decimal?>(optionValueType, new OptionValueConverter<decimal?>());
                case OptionValueType.NullableDateTime:
                case OptionValueType.NullableDate:
                case OptionValueType.NullableTime:
                    return new OptionValue<DateTime?>(optionValueType, new OptionValueConverter<DateTime?>());
                case OptionValueType.NullableTimeSpan:
                    return new OptionValue<TimeSpan?>(optionValueType, new OptionValueConverter<TimeSpan?>());
                case OptionValueType.ByteArray:
                    return new OptionValue<byte[]>(optionValueType, new OptionValueConverter<byte[]>());
                default:
                    throw new InvalidEnumArgumentException(nameof(optionValueType), (int)optionValueType, typeof(OptionValueType));
            }
        }
    }
}