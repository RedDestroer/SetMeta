using NUnit.Framework;
using SetMeta.Behaviours;
using SetMeta.Impl;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    public class SqlFixedListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void SqlFixedListOptionBehaviour_WhenWePassNullOptionValue_ThrowException()
        {
            void Delegate()
            {
                new SqlFixedListOptionBehaviour(null, "");
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionValue");
        }

        [Test]
        public void SqlFixedListOptionBehaviour_WhenWePassNullQuery_ThrowException()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            void Delegate()
            {
                new SqlFixedListOptionBehaviour(optionValue, null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "query");
        }

        [Test]
        public void SqlFixedListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = "Test Query";
            var value = "Test Value";
            var displayValue = "Test Display Value";

            var actual = new SqlFixedListOptionBehaviour(optionValue, query, value, displayValue);

            Assert.That(actual.Query, Is.EqualTo(query));
            Assert.That(actual.ValueMember, Is.EqualTo(value));
            Assert.That(actual.DisplayMember, Is.EqualTo(displayValue));
        }
    }
}