using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;
using RangedMinElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.RangedMinElement;

namespace SetMeta.Tests.TestDataCreators
{
    public class RangedMinBehaviourTestDataCreator
        : IRangedMinBehaviourTestDataCreator
    {
        public XElement Build(string min)
        {
            return new XElement(
                RangedMinElement.ElementName,
                new XAttribute(RangedMinElement.Attrs.Min, min));
        }
    }
}