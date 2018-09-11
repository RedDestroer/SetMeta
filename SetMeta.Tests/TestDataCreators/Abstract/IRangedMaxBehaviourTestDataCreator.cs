using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IRangedMaxBehaviourTestDataCreator
    {
        XElement Build(string max);
    }
}