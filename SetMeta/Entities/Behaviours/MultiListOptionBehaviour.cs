using System.Collections.Generic;
using SetMeta.Abstract;
using SetMeta.Util;

namespace SetMeta.Entities.Behaviours
{
    public class MultiListOptionBehaviour
        : OptionBehaviour
    {
        internal MultiListOptionBehaviour(IOptionValue optionValue, IEnumerable<ListItem> listItems, bool sorted = false, string separator = ";")
            : base(optionValue)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Validate.NotNull(listItems, nameof(listItems));
            Validate.NotNull(separator, nameof(separator));

            // ReSharper disable once PossibleMultipleEnumeration
            ListItems = new List<ListItem>(listItems);
            Sorted = sorted;
            Separator = separator;
        }

        public List<ListItem> ListItems { get; }
        public bool Sorted { get; }
        public string Separator { get; }
    }
}