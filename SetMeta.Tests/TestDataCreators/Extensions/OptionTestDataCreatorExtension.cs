using System.Xml.Linq;
using SetMeta.Tests.TestDataCreators.Abstract;

namespace SetMeta.Tests.TestDataCreators.Extensions
{
    public static class OptionTestDataCreatorExtension
    {
        public static XElement BuildNew(this IOptionTestDataCreator tdc, AutoFixtureBase autoFixture)
        {
            tdc.WithDisplayName(autoFixture.Fake<string>());
            tdc.WithDescription(autoFixture.Fake<string>());
            tdc.WithValueType(autoFixture.Fake<OptionValueType>());

            return tdc.Build(autoFixture.Fake("_"));
        }

        public static XElement BuildNew(this IOptionTestDataCreator tdc, AutoFixtureBase autoFixture, XElement element)
        {
            tdc.WithValueType(autoFixture.Fake<OptionValueType>());
            tdc.WithElement(element);

            return tdc.Build(autoFixture.Fake("_"));
        }
    }
}