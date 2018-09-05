namespace SetMeta.Tests.TestDataCreators
{
    public interface ISetMetaTestDataCreator
    {
        IOptionSetV1TestDataCreator OptionSet { get; }
        IOptionTestDataCreator Option { get; }
        IRangedMinMaxBehaviourTestDataCreator RangedMinMaxBehaviour { get; }
        IRangedMaxBehaviourTestDataCreator RangedMaxBehaviour { get; }
        IRangedMinBehaviourTestDataCreator RangedMinBehaviour { get; }
        IFixedListBehaviourTestDataCreator FixedListBehaviour { get; }
        ISqlFixedListBehaviourTestDataCreator SqlFixedListBehaviour { get; }
        IFlagListBehaviourTestDataCreator FlagListBehaviour { get; }
        ISqlFlagListBehaviourTestDataCreator SqlFlagListBehaviour { get; }
        IMultiListBehaviourTestDataCreator MultiListBehaviour { get; }
        ISqlMultiListBehaviourTestDataCreator SqlMultiListBehaviour { get; }
    }
}