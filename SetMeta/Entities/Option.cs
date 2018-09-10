using SetMeta.Abstract;

namespace SetMeta.Entities
{
    public class Option
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public object DefaultValue { get; set; }
        public OptionValueType ValueType { get; set; }
        public OptionBehaviour Behaviour { get; set; }
    }
}
