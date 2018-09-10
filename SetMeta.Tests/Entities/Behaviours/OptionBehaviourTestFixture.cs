using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class OptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void Value_ShouldReturnTheSameString_WhenWePassString()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            var sut = new OptionBehaviourTest(optionValue);

            object expected = Fake<string>();

            var actual = sut.Value((string)expected);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void StringValue_ShouldReturnTheSameString_WhenWePassString()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            var sut = new OptionBehaviourTest(optionValue);

            object expected = Fake<string>();

            var actual = sut.StringValue(expected);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldNotAcceptNullArgumentsForAllConstructors()
        {
            typeof(OptionBehaviourTest).ShouldNotAcceptNullConstructorArguments(AutoFixture);
        }

        private class OptionBehaviourTest
            : OptionBehaviour
        {
            public OptionBehaviourTest(IOptionValue optionValue) 
                : base(optionValue)
            {
            }
        }
    }
}
