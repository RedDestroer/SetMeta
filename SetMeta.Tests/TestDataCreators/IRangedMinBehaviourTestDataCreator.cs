using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public interface IRangedMinBehaviourTestDataCreator
    {
        XElement Build(string min);
    }
}