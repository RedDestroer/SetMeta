using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IRangedMinMaxBehaviourTestDataCreator
    {
        XElement Build(string min, string max);
    }
}