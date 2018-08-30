using System;
using System.IO;
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
        public void Create_WhenWePassStringNull_ThrowException()        
        {
            void Delegate()
            {
                OptionSetParser.Create((string)null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "version");
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
        public void Create_WhenWePassStreamNull_ThrowException()
        {
            void Delegate()
            {
                OptionSetParser.Create((Stream)null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "stream");
        }

        [Test]
        public void Create_WhenWePassReaderNull_ThrowException()
        {
            void Delegate()
            {
                OptionSetParser.Create((XmlTextReader)null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "reader");
        }

        [Test]
        public void Parse_WhenNullStreamIsPassed_Throws()
        {
            var sut = Dep<Mock<OptionSetParser>>()
                .Object;

            void Delegate()
            {
                sut.Parse((Stream)null, Fake<IOptionSetValidator>());
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "stream");
        }

        [Test]
        public void Parse_WhenNullIOptionSetValidatorIsPassed_Throws()
        {
            var sut = Dep<Mock<OptionSetParser>>()
                .Object;

            void Delegate()
            {
                sut.Parse(Fake<Stream>(), null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionSetValidator");
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

            void Delegate()
            {
                sut.Parse((string)null, Fake<IOptionSetValidator>());
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "data");
        }

        [Test]
        public void Parse_WhenNullIOptionSetValidatorIsPassedWithString_Throws()
        {
            var sut = Dep<Mock<OptionSetParser>>()
                .Object;

            void Delegate()
            {
                sut.Parse(Fake<string>(), null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionSetValidator");
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
