using System;
using System.Collections.Generic;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class FlagListOptionBehaviour
        : OptionBehaviour
    {
        internal FlagListOptionBehaviour(IOptionValue optionValue, IEnumerable<ListItem> listItems)
            : base(optionValue)
        {
            if (listItems == null) throw new ArgumentNullException(nameof(listItems));

            ListItems = new List<ListItem>(listItems);
        }

        public List<ListItem> ListItems { get; }
    }
}