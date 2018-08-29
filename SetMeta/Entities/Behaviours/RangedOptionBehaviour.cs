using System;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class RangedOptionBehaviour
        : OptionBehaviour
    {
        internal RangedOptionBehaviour(IOptionValue optionValue, string minValue, string maxValue)
            : base(optionValue)
        {
            IsMinValueExists = true;
            MinValue = minValue ?? throw new ArgumentNullException(nameof(minValue));
            IsMaxValueExists = true;
            MaxValue = maxValue ?? throw new ArgumentNullException(nameof(maxValue));
        }

        internal RangedOptionBehaviour(IOptionValue optionValue, string value, bool isMin)
            : base(optionValue)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (isMin)
            {
                IsMinValueExists = true;
                MinValue = value;
                IsMaxValueExists = false;
                MaxValue = null;
            }
            else
            {
                IsMinValueExists = false;
                MinValue = null;
                IsMaxValueExists = true;
                MaxValue = value;
            }
        }

        public string MinValue { get; }
        public bool IsMinValueExists { get; }
        public string MaxValue { get; }
        public bool IsMaxValueExists { get; }
    }
}