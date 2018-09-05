using System.Xml.Linq;
using SetMeta.Entities;

namespace SetMeta.Tests.TestDataCreators
{
    public class RangedMinBehaviourTestDataCreator
        : IRangedMinBehaviourTestDataCreator
    {
        public XElement Build(string min)
        {
            return new XElement(
                RangedMinBehaviourKeys.Name,
                new XAttribute(RangedMinBehaviourKeys.AttrKeys.Min, min));
        }
    }
}