using System;
using System.Xml;
using System.Xml.Schema;
using AutoFixture;
using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Impl;
using SetMeta.Tests.Util;
using XsdIterator;

namespace SetMeta.Tests.Impl
{
    [TestFixture, Ignore("Not competed")]
    internal class OptionSetParserV1XsdTestFixture
        : SutBase<OptionSetParserV1, OptionSetParser>
    {
        private static readonly Lazy<IOptionInformant> OptionInformant;
        private IOptionValueFactory _optionValueFactory = new OptionValueFactory();

        static OptionSetParserV1XsdTestFixture()
        {
            OptionInformant = new Lazy<IOptionInformant>(() =>
            {
                using (var reader = new XmlTextReader(StaticResources.GetStream("OptionSetV1.xsd")))
                {
                    var xmlSchema = XmlSchema.Read(reader, null);
                    return TraverseXmlSchema(xmlSchema);
                }
            });
        }

        protected override void SetUpInner()
        {
            _optionValueFactory = AutoFixture.Create<IOptionValueFactory>();
            base.SetUpInner();
        }

        private static IOptionInformant TraverseXmlSchema(XmlSchema xmlSchema)
        {
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(xmlSchema);
            schemaSet.Compile();

            var visitor = new OptionSetV1XmlSchemaProcessor();
            var iterator = new DefaultXmlSchemaIterator(schemaSet, visitor);

            var enumerator = schemaSet.GlobalElements.Values.GetEnumerator();
            enumerator.MoveNext();
            var globalElement = enumerator.Current;
            globalElement.Accept(iterator);

            return visitor;
        }
    }
}