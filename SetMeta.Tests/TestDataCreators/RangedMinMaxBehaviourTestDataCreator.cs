using System.Xml.Linq;
using SetMeta.Entities;

namespace SetMeta.Tests.TestDataCreators
{
    public class RangedMinMaxBehaviourTestDataCreator
        : IRangedMinMaxBehaviourTestDataCreator
    {
        public XElement Build(string min, string max)
        {
            return new XElement(
                RangedMinMaxBehaviourKeys.Name, 
                new XAttribute(RangedMinMaxBehaviourKeys.AttrKeys.Min, min),
                new XAttribute(RangedMinMaxBehaviourKeys.AttrKeys.Max, max));
        }
    }
}