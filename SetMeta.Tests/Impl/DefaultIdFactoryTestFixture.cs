using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Impl;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    public class DefaultIdFactoryTestFixture
        : SutBase<DefaultIdFactory, IIdFactory>
    {
        [Test]
        public void CreateId_ShouldReturnExpectedKey()
        {
            var data = Fake<string>();

            var expected = data.ToLowerInvariant();
            var actual = Sut.CreateId(data);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateId_ShouldReturnEmptyString_WhenDataIsNull()
        {
            var actual = Sut.CreateId(null);

            Assert.That(actual, Is.Empty);
        }
    }
}