namespace SetMeta.Entities
{
    public class Constant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public string ValueType { get; set; }
    }

    internal static class ConstantAttributeKeys
    {
        public const string Name = "name";
        public const string Value = "value";
        public const string ValueType = "valueType";
    }
}