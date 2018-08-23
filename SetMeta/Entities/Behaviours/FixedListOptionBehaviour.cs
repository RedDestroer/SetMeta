using System.Collections.Generic;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class FixedListOptionBehaviour
        : OptionBehaviour
    {
        internal FixedListOptionBehaviour(IOptionValue optionValue, IEnumerable<ListItem> validItems)
            : base(optionValue)
        {
            ListItems = new List<ListItem>(validItems);
        }

        public List<ListItem> ListItems { get; }
    }
}