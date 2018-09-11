using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators.Abstract
{
    public interface IRangedMinBehaviourTestDataCreator
    {
        XElement Build(string min);
    }
}