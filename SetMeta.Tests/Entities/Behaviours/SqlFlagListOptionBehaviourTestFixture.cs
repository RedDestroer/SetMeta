using NUnit.Framework;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class SqlFlagListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void SqlFlagListOptionBehaviour_WhenWePassNullOptionValue_ThrowException()
        {
            void Delegate()
            {
                new SqlFlagListOptionBehaviour(null, "");
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionValue");
        }

        [Test]
        public void SqlFlagListOptionBehaviour_WhenWePassNullQuery_ThrowException()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            void Delegate()
            {
                new SqlFlagListOptionBehaviour(optionValue, null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "query");
        }

        [Test]
        public void SqlFlagListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = "Test Query";
            var value = "Test Value";
            var displayValue = "Test Display Value";

            var actual = new SqlFlagListOptionBehaviour(optionValue, query, value, displayValue);

            Assert.That(actual.Query, Is.EqualTo(query));
            Assert.That(actual.ValueMember, Is.EqualTo(value));
            Assert.That(actual.DisplayMember, Is.EqualTo(displayValue));
        }
    }
}