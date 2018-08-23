using SetMeta.Abstract;

namespace SetMeta.Entities.Behaviours
{
    public class SimpleOptionBehaviour
        : OptionBehaviour
    {
        public SimpleOptionBehaviour(IOptionValue optionValue)
            : base(optionValue)
        {
        }       
    }
}