using System.Collections.Generic;
using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class MultiListOptionBehaviour
        : OptionBehaviour
    {
        internal MultiListOptionBehaviour(IOptionValue optionValue, IEnumerable<ListItem> validItems, bool sorted = false, string separator = ";")
            : base(optionValue)
        {
            ListItems = new List<ListItem>(validItems);
            Sorted = sorted;
            Separator = separator;
        }

        public List<ListItem> ListItems { get; }
        public bool Sorted { get; }
        public string Separator { get; }
    }
}