using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class RangedOptionBehaviour
        : OptionBehaviour
    {
        public RangedOptionBehaviour(IOptionValue optionValue, string minValue, string maxValue)
            : base(optionValue)
        {
            IsMinValueExists = true;
            MinValue = minValue;
            IsMaxValueExists = true;
            MaxValue = maxValue;
        }

        public RangedOptionBehaviour(IOptionValue optionValue, string value, bool isMin)
            : base(optionValue)
        {
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