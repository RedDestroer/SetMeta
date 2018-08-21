using NUnit.Framework;
using SetMeta.Behaviours;
using SetMeta.Impl;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    public class SqlMultiListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void SqlMultiListOptionBehaviour_WhenWePassNullOptionValue_ThrowException()
        {
            void Delegate()
            {
                new SqlMultiListOptionBehaviour(null, "");
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionValue");
        }

        [Test]
        public void SqlMultiListOptionBehaviour_WhenWePassNullQuery_ThrowException()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            void Delegate()
            {
                new SqlMultiListOptionBehaviour(optionValue, null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "query");
        }

        [Test]
        public void SqlMultiListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = "Test Query";
            var separator = "/";
            var value = "Test Value";
            var displayValue = "Test Display Value";

            var actual = new SqlMultiListOptionBehaviour(optionValue, query, true, separator, value, displayValue);

            Assert.That(actual.Query, Is.EqualTo(query));
            Assert.That(actual.Sorted, Is.True);
            Assert.That(actual.Separator, Is.EqualTo(separator));
            Assert.That(actual.ValueMember, Is.EqualTo(value));
            Assert.That(actual.DisplayMember, Is.EqualTo(displayValue));
        }
    }
}