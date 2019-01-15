using System;
using System.Collections.Generic;
using System.Xml;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SetMeta.Impl;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    public class DefaultOptionSetValidatorTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void AddError_WhenWePassEmptyMessage_ThrowException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var target = AutoFixture.Freeze<DefaultOptionSetValidator>();
                target.AddError(null, null);
            });

            Assert.That(ex.ParamName, Is.EqualTo("message"));
        }

        [Test]
        public void ValidationResults_NotNullAndEmptyAfterCreation()
        {
            var target = AutoFixture.Freeze<DefaultOptionSetValidator>();

            Assert.That(target.ValidationResults, Is.Not.Null);
            Assert.That(target.ValidationResults, Is.Empty);
        }

        [Test]
        public void AddError_WhenCallAddError_MessagePersistsAmongValidationResults()
        {
            var message = AutoFixture.Create<string>();
            var xmlLineInfo = AutoFixture.Create<Mock<IXmlLineInfo>>()
                                         .Object;

            var target = AutoFixture.Freeze<DefaultOptionSetValidator>();
            target.AddError(message, xmlLineInfo);

            Assert.That(target.ValidationResults, Is.Not.Null);
            Assert.That(target.ValidationResults.Count, Is.EqualTo(1));
            Assert.That(target.ValidationResults[0].Key, Is.EqualTo(message));
            Assert.That(target.ValidationResults[0].Value, Is.SameAs(xmlLineInfo));
        }

        [Test]
        public void AddError_WhenCallAddErrorTwice_MessagesPersistsAmongValidationResults()
        {
            var message1 = AutoFixture.Create<string>();
            var message2 = AutoFixture.Create<string>();
            var expected = new List<KeyValuePair<string, IXmlLineInfo>>
            {
                new KeyValuePair<string, IXmlLineInfo>(message1, null),
                new KeyValuePair<string, IXmlLineInfo>(message2, null)
            };

            var target = AutoFixture.Freeze<DefaultOptionSetValidator>();
            target.AddError(message1);
            target.AddError(message2);

            Assert.That(target.ValidationResults, Is.Not.Null);
            Assert.That(target.ValidationResults, Is.EquivalentTo(expected));
        }
    }
}