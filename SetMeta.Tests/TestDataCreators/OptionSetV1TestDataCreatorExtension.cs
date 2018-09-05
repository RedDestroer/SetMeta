using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public static class OptionSetV1TestDataCreatorExtension
    {
        public static XDocument BuildNew(this IOptionSetV1TestDataCreator tdc, AutoFixtureBase autoFixture)
        {
            tdc.WithElement(autoFixture.TestDataCreator.Option.BuildNew(autoFixture));
            tdc.WithElement(autoFixture.TestDataCreator.Option.BuildNew(autoFixture));
            tdc.WithElement(autoFixture.TestDataCreator.Option.BuildNew(autoFixture));

            return tdc.Build();
        }

        public static XDocument BuildNew(this IOptionSetV1TestDataCreator tdc, XElement element)
        {
            tdc.WithElement(element);

            return tdc.Build();
        }
    }
}