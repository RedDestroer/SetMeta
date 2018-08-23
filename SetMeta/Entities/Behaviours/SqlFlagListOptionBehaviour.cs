using System;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class SqlFlagListOptionBehaviour
        : OptionBehaviour
    {
        internal SqlFlagListOptionBehaviour(IOptionValue optionValue, string query, string valueMember = "value", string displayMember = "displayValue")
            : base(optionValue)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
            ValueMember = valueMember;
            DisplayMember = displayMember;
        }

        public string Query { get; }
        public string ValueMember { get; }
        public string DisplayMember { get; }
    }
}