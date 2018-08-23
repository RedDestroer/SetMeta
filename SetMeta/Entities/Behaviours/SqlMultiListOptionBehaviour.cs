using System;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
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

        public bool Sorted { get; }
        public string Separator { get; }
        public string Query { get; }
        public string ValueMember { get; }
        public string DisplayMember { get; }
    }
}