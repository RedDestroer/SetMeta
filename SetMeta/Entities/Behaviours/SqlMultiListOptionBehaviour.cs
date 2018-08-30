using System;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class SqlMultiListOptionBehaviour
        : OptionBehaviour
    {
        internal SqlMultiListOptionBehaviour(IOptionValue optionValue, string query, bool sorted, string separator, string valueMember, string displayMember)
            : base(optionValue)
        {
            Sorted = sorted;
            Separator = separator ?? throw new ArgumentNullException(nameof(separator));
            Query = query ?? throw new ArgumentNullException(nameof(query));
            ValueMember = valueMember ?? throw new ArgumentNullException(nameof(valueMember));
            DisplayMember = displayMember ?? throw new ArgumentNullException(nameof(displayMember));
        }

        public bool Sorted { get; }
        public string Separator { get; }
        public string Query { get; }
        public string ValueMember { get; }
        public string DisplayMember { get; }
    }
}