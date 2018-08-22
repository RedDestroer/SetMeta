using OptionBehaviourDao = SetMeta.Dao.OptionBehavior;
using OptionBehaviourEntity = SetMeta.Abstract.OptionBehaviour;

namespace SetMeta.Abstract
{
    public interface IOptionBehaviourFactory
    {
        OptionBehaviourEntity Create(OptionBehaviourDao optionBehavior, IOptionValue optionValue);
    }
}