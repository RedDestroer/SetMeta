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
        IListItemTestDataCreator ListItemTestDataCreator { get; }
        ISqlFixedListBehaviourTestDataCreator SqlFixedListBehaviour { get; }
        IFlagListBehaviourTestDataCreator FlagListBehaviour { get; }
        ISqlFlagListBehaviourTestDataCreator SqlFlagListBehaviour { get; }
        IMultiListBehaviourTestDataCreator MultiListBehaviour { get; }
        ISqlMultiListBehaviourTestDataCreator SqlMultiListBehaviour { get; }
        IGroupTestDataCreator Group { get; }
        IGroupOptionTestDataCreator GroupOption { get; }
        IOptionSuggestionTestDataCreator OptionSuggestion { get; }
        IConstantTestDataCreator Constant { get; }
        ISuggestionTestDataCreator Suggestion { get; }
    }
}