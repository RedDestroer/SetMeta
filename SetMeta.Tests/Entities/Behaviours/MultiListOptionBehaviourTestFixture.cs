using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class MultiListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void MultiListOptionBehaviour_ConstructorNullChecks()
        {
            typeof(MultiListOptionBehaviour).ShouldNotAcceptNullConstructorArguments(AutoFixture, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void MultiListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = FakeMany<ListItem>();

            var actual = new MultiListOptionBehaviour(optionValue, list);

            Assert.That(actual.ListItems, Is.EqualTo(list));
        }

        [TestCase(true, "/")]
        [TestCase(true, ";")]
        [TestCase(false, "/")]
        public void MultiListOptionBehaviour_WhenWePassSortedAndSeparator_TheyAssignedCorrectly(bool sorted, string separator)
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = Fake<List<ListItem>>();

            var actual = new MultiListOptionBehaviour(optionValue, list, sorted, separator);

            Assert.That(actual.Sorted, Is.EqualTo(sorted));
            Assert.That(actual.Separator, Is.EqualTo(separator));
        }
    }
}