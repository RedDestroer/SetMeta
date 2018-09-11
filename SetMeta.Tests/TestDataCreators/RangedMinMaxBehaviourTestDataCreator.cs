using System.Xml.Linq;
using RangedMinMaxElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.RangedMinMaxElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class RangedMinMaxBehaviourTestDataCreator
        : IRangedMinMaxBehaviourTestDataCreator
    {
        public XElement Build(string min, string max)
        {
            return new XElement(
                RangedMinMaxElement.ElementName, 
                new XAttribute(RangedMinMaxElement.Attrs.Min, min),
                new XAttribute(RangedMinMaxElement.Attrs.Max, max));
        }
    }
}