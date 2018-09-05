using System.Xml.Linq;
using SetMeta.Entities;

namespace SetMeta.Tests.TestDataCreators
{
    public class RangedMaxBehaviourTestDataCreator
        : IRangedMaxBehaviourTestDataCreator
    {
        public XElement Build(string max)
        {
            return new XElement(
                RangedMaxBehaviourKeys.Name,
                new XAttribute(RangedMaxBehaviourKeys.AttrKeys.Max, max));
        }
    }
}