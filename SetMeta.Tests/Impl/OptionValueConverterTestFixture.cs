using System.Globalization;
using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    internal class OptionValueConverterTestFixture
        : SutBase<OptionValueConverter<string>, IOptionValueConverter<string>>
    {
        [Test]
        public void ShouldNotAcceptNullArgumentsForAllConstructors()
        {
            ShouldNotAcceptNullArgumentsForAllConstructorsInner();
        }

        [Test]
        public void GetValue_WhenWePassString_ThereShouldBeNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var actual = Sut.GetValue(Fake<string>());

                Assert.IsNotNull(actual);
            });
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetValue_WhenWePassNullOrEmptyString_NullOrEmptyStringShouldBeReturned(string value)
        {
            var actual = Sut.GetValue(value);

            Assert.That(actual, Is.EqualTo(value));
        }

        [Test]
        public void GetValue_WhenWePassString_SameStringShouldBeReturned()
        {

            string expected = Fake<string>();

            var actual = Sut.GetValue(expected);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetStringValue_WhenWePassString_ThereShouldBeNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var actual = Sut.GetStringValue(Fake<string>());

                Assert.IsNotNull(actual);
            });
        }

        [TestCase("")]
        [TestCase(null)]
        public void GetStringValue_WhenWePassNullOrEmptyString_NullOrEmptyStringShouldBeReturned(string value)
        {
            var actual = Sut.GetStringValue(value);

            Assert.That(actual, Is.EqualTo(value));
        }

        [Test]
        public void GetStringValue_WhenWePassString_SameStringShouldBeReturned()
        {
            string expected = Fake<string>();

            var actual = Sut.GetStringValue(expected);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetStringValue_WhenFormatProviderIsNull_ThrowException()
        {
            Sut.ShouldThrowArgumentNullException(o => o.GetStringValue(Fake<string>(), null), "formatProvider");
        }

        [TestCase("", "ru-RU")]
        [TestCase(null, "ru-RU")]
        [TestCase("", "en-US")]
        [TestCase(null, "en-US")]
        public void GetStringValueCulture_WhenWePassNullOrEmptyString_NullOrEmptyStringShouldBeReturned(string expected, string cultureName)
        {
            var actual = Sut.GetStringValue(expected, CreateCultureInfo(cultureName));

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("ru-RU")]
        [TestCase("en-US")]
        public void GetStringValueCulture_WhenWePassString_SameStringShouldBeReturned(string cultureName)
        {
            string expected = Fake<string>();

            var actual = Sut.GetStringValue(expected, CreateCultureInfo(cultureName));

            Assert.That(actual, Is.EqualTo(expected));
        }

        private CultureInfo CreateCultureInfo(string value)
        {
            return new CultureInfo(value);
        }
    }
}
