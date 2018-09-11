using System.Xml.Linq;
using RangedMaxElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.RangedMaxElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class RangedMaxBehaviourTestDataCreator
        : IRangedMaxBehaviourTestDataCreator
    {
        public XElement Build(string max)
        {
            return new XElement(
                RangedMaxElement.ElementName,
                new XAttribute(RangedMaxElement.Attrs.Max, max));
        }
    }
}