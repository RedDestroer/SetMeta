using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IRangedMaxBehaviourTestDataCreator
    {
        XElement Build(string max);
    }
}