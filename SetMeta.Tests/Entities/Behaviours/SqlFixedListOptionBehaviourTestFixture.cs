using System.Reflection;
using NUnit.Framework;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class SqlFixedListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void SqlFixedListOptionBehaviour_ConstructorNullChecks()
        {
            typeof(SqlFixedListOptionBehaviour).ShouldNotAcceptNullConstructorArguments(AutoFixture, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void SqlFixedListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = Fake<string>();
            var value = Fake<string>();
            var displayValue = Fake<string>();

            var actual = new SqlFixedListOptionBehaviour(optionValue, query, value, displayValue);

            Assert.That(actual.Query, Is.EqualTo(query));
            Assert.That(actual.ValueMember, Is.EqualTo(value));
            Assert.That(actual.DisplayMember, Is.EqualTo(displayValue));
        }
    }
}