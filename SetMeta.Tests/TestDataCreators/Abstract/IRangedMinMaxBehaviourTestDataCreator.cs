using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IRangedMinMaxBehaviourTestDataCreator
    {
        XElement Build(string min, string max);
    }
}