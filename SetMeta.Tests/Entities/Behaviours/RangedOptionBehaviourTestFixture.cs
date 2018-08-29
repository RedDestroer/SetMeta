using System.Reflection;
using NUnit.Framework;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class RangedOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void RangedOptionBehaviour_ConstructorNullChecks()
        {
            typeof(RangedOptionBehaviour).ShouldNotAcceptNullConstructorArguments(AutoFixture, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void RangedOptionBehaviour_WhenWePassMinAndMax_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var maxValue = Fake<string>();
            var minValue = Fake<string>();

            var actual = new RangedOptionBehaviour(optionValue, minValue, maxValue);

            Assert.That(actual.IsMaxValueExists, Is.True);
            Assert.That(actual.IsMinValueExists, Is.True);
            Assert.That(actual.MaxValue, Is.EqualTo(maxValue));
            Assert.That(actual.MinValue, Is.EqualTo(minValue));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void RangedOptionBehaviour_WhenWePassValueAndBool_TheyAssignedCorrectly(bool isMin)
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var value = Fake<string>();

            var actual = new RangedOptionBehaviour(optionValue, value, isMin);

            Assert.That(actual.IsMinValueExists, Is.EqualTo(isMin));
            Assert.That(actual.IsMaxValueExists, Is.EqualTo(!isMin));

            if (isMin)
            {
                Assert.That(actual.MaxValue, Is.Null);
                Assert.That(actual.MinValue, Is.EqualTo(value));
            }
            else
            {
                Assert.That(actual.MinValue, Is.Null);
                Assert.That(actual.MaxValue, Is.EqualTo(value));
            }
        }
    }
}