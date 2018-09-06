namespace SetMeta.Tests.TestDataCreators
{
    public class SetMetaTestDataCreator
        : ISetMetaTestDataCreator
    {
        public SetMetaTestDataCreator()
        {
            OptionSet = new OptionSetV1TestDataCreator();
            Option = new OptionTestDataCreator();
            RangedMinMaxBehaviour = new RangedMinMaxBehaviourTestDataCreator();
            RangedMaxBehaviour = new RangedMaxBehaviourTestDataCreator();
            RangedMinBehaviour = new RangedMinBehaviourTestDataCreator();
            FixedListBehaviour = new FixedListBehaviourTestDataCreator();
            SqlFixedListBehaviour = new SqlFixedListBehaviourTestDataCreator();
            FlagListBehaviour = new FlagListBehaviourTestDataCreator();
            SqlFlagListBehaviour = new SqlFlagListBehaviourTestDataCreator();
            MultiListBehaviour = new MultiListBehaviourTestDataCreator();
            SqlMultiListBehaviour = new SqlMultiListBehaviourTestDataCreator();
            Group = new GroupTestDataCreator();
            GroupOption = new GroupOptionTestDataCreator();
            OptionSuggestion = new OptionSuggestionTestDataCreator();
            Constant = new ConstantTestDataCreator();
            Suggestion = new SuggestionTestDataCreator();
        }

        public IOptionSetV1TestDataCreator OptionSet { get; }
        public IOptionTestDataCreator Option { get; }
        public IRangedMinMaxBehaviourTestDataCreator RangedMinMaxBehaviour { get; }
        public IRangedMaxBehaviourTestDataCreator RangedMaxBehaviour { get; }
        public IRangedMinBehaviourTestDataCreator RangedMinBehaviour { get; }
        public IFixedListBehaviourTestDataCreator FixedListBehaviour { get; }
        public ISqlFixedListBehaviourTestDataCreator SqlFixedListBehaviour { get; }
        public IFlagListBehaviourTestDataCreator FlagListBehaviour { get; }
        public ISqlFlagListBehaviourTestDataCreator SqlFlagListBehaviour { get; }
        public IMultiListBehaviourTestDataCreator MultiListBehaviour { get; }
        public ISqlMultiListBehaviourTestDataCreator SqlMultiListBehaviour { get; }
        public IGroupTestDataCreator Group { get; }
        public IGroupOptionTestDataCreator GroupOption { get; }
        public IOptionSuggestionTestDataCreator OptionSuggestion { get; }
        public IConstantTestDataCreator Constant { get; }
        public ISuggestionTestDataCreator Suggestion { get; }
    }
}