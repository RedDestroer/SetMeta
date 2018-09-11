using System.Reflection;
using NUnit.Framework;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class SqlMultiListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void ShouldNotAcceptNullArgumentsForAllConstructors()
        {
            typeof(SqlMultiListOptionBehaviour).ShouldNotAcceptNullConstructorArguments(AutoFixture, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void SqlMultiListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = Fake<string>();
            var separator = Fake<string>();
            var value = Fake<string>();
            var displayValue = Fake<string>();

            var actual = new SqlMultiListOptionBehaviour(optionValue, query, true, separator, value, displayValue);

            Assert.That(actual.Query, Is.EqualTo(query));
            Assert.That(actual.Sorted, Is.True);
            Assert.That(actual.Separator, Is.EqualTo(separator));
            Assert.That(actual.ValueMember, Is.EqualTo(value));
            Assert.That(actual.DisplayMember, Is.EqualTo(displayValue));
        }
    }
}