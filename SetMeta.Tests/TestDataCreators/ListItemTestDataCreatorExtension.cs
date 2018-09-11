using System.Collections.Generic;
using System.Xml.Linq;

namespace SetMeta.Tests.TestDataCreators
{
    public static class ListItemTestDataCreatorExtension
    {
        public static XElement BuildNew(this IListItemTestDataCreator tdc, AutoFixtureBase autoFixture)
        {
            tdc.WithValue(autoFixture.Fake<string>());
            tdc.WithDisplayValue(autoFixture.Fake<string>());

            return tdc.Build();
        }

        public static IEnumerable<XElement> BuildMany(this IListItemTestDataCreator tdc, AutoFixtureBase autoFixture, ushort count = 3)
        {
            for (int i = 0; i < count; i++)
            {
                yield return tdc.BuildNew(autoFixture);
            }
        }
    }
}