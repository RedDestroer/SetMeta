using System;
using SetMeta.Abstract;

namespace SetMeta.Behaviours
{
    public class SqlMultiListOptionBehaviour
        : OptionBehaviour
    {
        internal SqlMultiListOptionBehaviour(IOptionValue optionValue, string query, bool sorted = false, string separator = ";", string valueMember = "value", string displayMember = "displayValue")
            : base(optionValue)
        {
            Sorted = sorted;
            Separator = separator;
            Query = query ?? throw new ArgumentNullException(nameof(query));
            ValueMember = valueMember;
            DisplayMember = displayMember;
        }

        public bool Sorted { get; private set; }
        public string Separator { get; private set; }
        public string Query { get; private set; }
        public string ValueMember { get; private set; }
        public string DisplayMember { get; private set; }
    }
}