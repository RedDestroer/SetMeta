using System;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class SqlFixedListOptionBehaviour
        : OptionBehaviour
    {
        internal SqlFixedListOptionBehaviour(IOptionValue optionValue, string query, string valueMember, string displayMember)
            : base(optionValue)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
            ValueMember = valueMember ?? throw new ArgumentNullException(nameof(valueMember));
            DisplayMember = displayMember ?? throw new ArgumentNullException(nameof(displayMember));
        }

        public string Query { get; }
        public string ValueMember { get; }
        public string DisplayMember { get; }
    }
}