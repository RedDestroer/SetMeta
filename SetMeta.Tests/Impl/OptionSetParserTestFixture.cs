using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Moq;
using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    public class OptionSetParserTestFixture 
        : AutoFixtureBase
    {
        protected override void SetUpInner()
        {
            base.SetUpInner();
            AutoFixture.Customize(new StrictAutoMoqCustomization());
        }

        [Test]
        public void ShouldNotAcceptNullArgumentsForAllMethodsInner()
        {
            typeof(OptionSetParser).ShouldNotAcceptNullArgumentsForAllMethods(AutoFixture, mi => mi.Name != nameof(OptionSetParser.CreateId), BindingFlags.Public | BindingFlags.Static);
        }

        [Test]
        public void Create_WhenWePassEmptyString_ThrowException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                OptionSetParser.Create("");
            });

            Assert.That(ex.Message, Is.EqualTo($"Can't create '{nameof(OptionSetParser)}' of given version ''."));
        }


        [Test]
        public void Parse_WhenNullStreamIsPassed_Throws()
        {
            var sut = Dep<Mock<OptionSetParser>>()
                .Object;

            sut.ShouldThrowArgumentNullException(o => o.Parse((Stream)null, Fake<IOptionSetValidator>()), "stream");
        }

        [Test]
        public void Parse_WhenNullIOptionSetValidatorIsPassed_Throws()
        {
            var sut = Dep<Mock<OptionSetParser>>()
                .Object;

            sut.ShouldThrowArgumentNullException(o => o.Parse(Fake<Stream>(), null), "optionSetValidator");
        }

        [Test]
        public void Parse_WhenStreamIsPassed_ParseWithXmlTextReaderIsCalled()
        {
            var mock = Dep<Mock<OptionSetParser>>();
            mock.Setup(o => o.Parse(It.IsAny<XmlTextReader>(), It.IsNotNull<IOptionSetValidator>()))
                .Returns(() => null)
                .Verifiable();

            var sut = mock.Object;

            using (var stream = new MemoryStream())
            {
                sut.Parse(stream, Fake<IOptionSetValidator>());
            }
            
            mock.Verify(o => o.Parse(It.IsAny<XmlTextReader>(), It.IsNotNull<IOptionSetValidator>()), Times.Once());
        }

        [Test]
        public void Parse_WhenNullStringIsPassed_Throws()
        {
            var sut = Dep<Mock<OptionSetParser>>()
                .Object;

            sut.ShouldThrowArgumentNullException(o => o.Parse((string)null, Fake<IOptionSetValidator>()), "data");
        }

        [Test]
        public void Parse_WhenNullIOptionSetValidatorIsPassedWithString_Throws()
        {
            var sut = Dep<Mock<OptionSetParser>>()
                .Object;

            sut.ShouldThrowArgumentNullException(o => o.Parse(Fake<string>(), null), "optionSetValidator");
        }

        [Test]
        public void Parse_WhenStringIsPassed_ParseWithXmlTextReaderIsCalled()
        {
            var mock = Dep<Mock<OptionSetParser>>();
            mock.Setup(o => o.Parse(It.IsAny<XmlTextReader>(), It.IsNotNull<IOptionSetValidator>()))
                .Returns(() => null)
                .Verifiable();

            var sut = mock.Object;

            sut.Parse(Fake<string>(), Fake<IOptionSetValidator>());

            mock.Verify(o => o.Parse(It.IsAny<XmlTextReader>(), It.IsNotNull<IOptionSetValidator>()), Times.Once());
        }

        [Test]
        public void CreateId_WhenWePassStringNull_ReturnEmptyString()
        {
            var actual = OptionSetParser.CreateId(null);

            Assert.That(actual, Is.EqualTo(string.Empty));
        }
    }
}
