using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Impl;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class OptionBehaviourTestFixture
        :AutoFixtureBase
    {
        [Test]
        public void Value_WhenWePassString_SameStringShouldBeReturned()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            var sut = new OptionBehaviourTest(optionValue);

            object expected = Fake<string>();

            var actual = sut.Value((string)expected);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void StringValue_WhenWePassString_SameStringShouldBeReturned()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            var sut = new OptionBehaviourTest(optionValue);

            object expected = Fake<string>();

            var actual = sut.StringValue(expected);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void OptionBehaviour_WhenWePassNull_ThrowException()
        {
            void Delegate()
            {
                // ReSharper disable once ObjectCreationAsStatement
                new OptionBehaviourTest(null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionValue");
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
