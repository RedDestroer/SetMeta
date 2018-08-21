using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Behaviours;
using SetMeta.Entities;
using SetMeta.Impl;
using SetMeta.Tests.Util;
using SetMeta.Util;
using XsdIterator;
using Assert = NUnit.Framework.Assert;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    internal class OptionSetParserV1TestFixture
        : SutBase<OptionSetParserV1, OptionSetParser>
    {
        private static readonly Lazy<IOptionInformator> OptionInformator;

        static OptionSetParserV1TestFixture()
        {
            OptionInformator = new Lazy<IOptionInformator>(() =>
            {
                using (var reader = new XmlTextReader(StaticResources.GetStream("OptionSetV1.xsd")))
                {
                    var xmlSchema = XmlSchema.Read(reader, null);
                    return TraverseXmlSchema(xmlSchema);
                }
            });
        }

        [Test]
        public void OptionSetParserV1_WhenWePassNull_ThrowException()
        {
            void Delegate()
            {
                new OptionSetParserV1(null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionValueFactory");
        }

        [Test]
        public void Parse_WhenNullXmlTextReaderIsPassed_Throws()
        {
            void Delegate()
            {
                Sut.Parse((XmlTextReader)null);
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "reader");
        }

        [Test]
        public void Parse_WhenXmlHasNoBody_Throws()
        {
            Assert.Throws<XmlException>(() =>
            {
                using (var reader = CreateReader("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"))
                {
                    Sut.Parse(reader);
                }
            });
        }

        [Test]
        public void Parse_WithOnlyRequaredAttributes_ReturnNotNull()
        {
            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required);
            
            var actual = Sut.Parse(CreateReader(document));
            
            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));

            var expected = GetExpectedOptionSet(actual.Options[0]);

            actual.Should().BeEquivalentTo(expected);
        }

        [TestCase(OptionAttributeKeys.Name, typeof(string), nameof(Option.Name))]
        [TestCase(OptionAttributeKeys.DefaultValue, typeof(string), nameof(Option.DefaultValue))]
        [TestCase(OptionAttributeKeys.Description, typeof(string), nameof(Option.Description))]
        [TestCase(OptionAttributeKeys.DisplayName, typeof(string), nameof(Option.DisplayName))]
        [TestCase(OptionAttributeKeys.ValueType, typeof(OptionValueType), nameof(Option.ValueType))]
        [TestCategory("Option")]
        public void Parse_WhenItPresentInOption_ShouldReadAttribute(string attributeName, Type attributeValueType, string propertyName)
        {
            DataConversion.AddParser(delegate(string input, out object value)
            {
                value = input;
                return true;
            });

            var attributeValue = GetNextValue(attributeValueType);

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required || a.Name == attributeName, attributeName, attributeValue);

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));          

            var propertyInfo = typeof(Option).GetProperty(propertyName);
            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.GetValue(actual.Options[0]), Is.EqualTo(attributeValue));
        }

        [TestCase(OptionAttributeKeys.DefaultValue, typeof(string), nameof(Option.DefaultValue), OptionAttributeDefaults.DefaultValue)]
        [TestCase(OptionAttributeKeys.Description, typeof(string), nameof(Option.Description), OptionAttributeDefaults.Description)]
        [TestCase(OptionAttributeKeys.DisplayName, typeof(string), nameof(Option.DisplayName), OptionAttributeDefaults.DisplayName)]
        [TestCase(OptionAttributeKeys.ValueType, typeof(OptionValueType), nameof(Option.ValueType), OptionAttributeDefaults.ValueType)]
        [TestCategory("Option")]
        public void Parse_WhenItAbsentInOption_ShouldReturnDefaultValue(string attributeName, Type attributeValueType, string propertyName, object attributeValue)
        {
            DataConversion.AddParser(delegate (string input, out object value)
            {
                value = input;
                return true;
            });

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required);

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));

            var propertyInfo = typeof(Option).GetProperty(propertyName);
            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.GetValue(actual.Options[0]), Is.EqualTo(attributeValue));
        }

        [TestCase("Test max", "Test min", null)]
        [TestCase("Test max", null, false)]
        [TestCase(null, "Test min", true)]
        public void Parse_WhenItPresentRangedBehaviour_ShouldReturnCorrectBehaviour(string maxValue, string minValue, object isMin)
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateRangedBehaviourMinMax(optionValue, minValue, maxValue, isMin));

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options[0].Behaviour, Is.TypeOf<RangedOptionBehaviour>());

            var rangedOptionBehaviour = (RangedOptionBehaviour) actual.Options[0].Behaviour;

            Assert.That(rangedOptionBehaviour.MaxValue, Is.EqualTo(maxValue));
            Assert.That(rangedOptionBehaviour.MinValue, Is.EqualTo(minValue));

            if (isMin == null)
            {
                Assert.That(rangedOptionBehaviour.IsMaxValueExists, Is.True);
                Assert.That(rangedOptionBehaviour.IsMinValueExists, Is.True);
            }else if (!(bool)isMin)
            {
                Assert.That(rangedOptionBehaviour.IsMaxValueExists, Is.True);
                Assert.That(rangedOptionBehaviour.IsMinValueExists, Is.False);
            }
            else if ((bool)isMin)
            {
                Assert.That(rangedOptionBehaviour.IsMaxValueExists, Is.False);
                Assert.That(rangedOptionBehaviour.IsMinValueExists, Is.True);
            }
        }

        [Test]
        public void Parse_WhenItPresentFixedListBehaviour_ShouldReturnCorrectBehaviour()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = new List<ListItem>
            {
                new ListItem
                {
                    Value = "Value 1",
                    DisplayValue = "Display Value 1"
                },
                new ListItem
                {
                    Value = "Value 2",
                    DisplayValue = "Display Value 2"
                },
                new ListItem
                {
                    Value = "Value 3",
                    DisplayValue = "Display Value 3"
                },
                new ListItem
                {
                    Value = "Value 4",
                    DisplayValue = "Display Value 4"
                },
            };

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateFixedListBehaviour(optionValue, list));

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options[0].Behaviour, Is.TypeOf<FixedListOptionBehaviour>());

            var fixedListOptionBehaviour = (FixedListOptionBehaviour) actual.Options[0].Behaviour;

            Assert.That(fixedListOptionBehaviour.ListItems, Is.EqualTo(list));

        }


        [Test]
        public void Parse_WhenItPresentFlagListBehaviour_ShouldReturnCorrectBehaviour()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = new List<ListItem>
            {
                new ListItem
                {
                    Value = "Value 1",
                    DisplayValue = "Display Value 1"
                },
                new ListItem
                {
                    Value = "Value 2",
                    DisplayValue = "Display Value 2"
                },
                new ListItem
                {
                    Value = "Value 3",
                    DisplayValue = "Display Value 3"
                },
                new ListItem
                {
                    Value = "Value 4",
                    DisplayValue = "Display Value 4"
                },
            };

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateFlagListBehaviour(optionValue, list));

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options[0].Behaviour, Is.TypeOf<FlagListOptionBehaviour>());

            var flagListOptionBehaviour = (FlagListOptionBehaviour)actual.Options[0].Behaviour;

            Assert.That(flagListOptionBehaviour.ListItems, Is.EqualTo(list));

        }

        [TestCase(true, "/")]
        [TestCase(true, ";")]
        [TestCase(false, "/")]
        public void Parse_WhenItPresentMultiListBehaviour_ShouldReturnCorrectBehaviour(bool sorted, string separator)
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = new List<ListItem>
            {
                new ListItem
                {
                    Value = "Value 1",
                    DisplayValue = "Display Value 1"
                },
                new ListItem
                {
                    Value = "Value 2",
                    DisplayValue = "Display Value 2"
                },
                new ListItem
                {
                    Value = "Value 3",
                    DisplayValue = "Display Value 3"
                },
                new ListItem
                {
                    Value = "Value 4",
                    DisplayValue = "Display Value 4"
                },
            };

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateMultiListBehaviour(optionValue, list, sorted, separator));

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options[0].Behaviour, Is.TypeOf<MultiListOptionBehaviour>());

            var multiListOptionBehaviour = (MultiListOptionBehaviour)actual.Options[0].Behaviour;

            Assert.That(multiListOptionBehaviour.ListItems, Is.EqualTo(list));
            Assert.That(multiListOptionBehaviour.Sorted, Is.EqualTo(sorted));
            Assert.That(multiListOptionBehaviour.Separator, Is.EqualTo(separator));

        }

        [Test]
        public void Parse_WhenItPresentSqlFixedListBehaviour_ShouldReturnCorrectBehaviour()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = Fake<string>();
            var memberValue = Fake<string>();
            var displayValue = Fake<string>();

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateSqlFixedListBehaviour(optionValue, query, memberValue, displayValue));

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options[0].Behaviour, Is.TypeOf<SqlFixedListOptionBehaviour>());

            var sqlFixedListOptionBehaviour = (SqlFixedListOptionBehaviour)actual.Options[0].Behaviour;

            Assert.That(sqlFixedListOptionBehaviour.Query, Is.EqualTo(query));
            Assert.That(sqlFixedListOptionBehaviour.ValueMember, Is.EqualTo(memberValue));
            Assert.That(sqlFixedListOptionBehaviour.DisplayMember, Is.EqualTo(displayValue));
        }

        [Test]
        public void Parse_WhenItPresentSqlFlagListBehaviour_ShouldReturnCorrectBehaviour()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = Fake<string>();
            var memberValue = Fake<string>();
            var displayValue = Fake<string>();

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateSqlFlagListBehaviour(optionValue, query, memberValue, displayValue));

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options[0].Behaviour, Is.TypeOf<SqlFlagListOptionBehaviour>());

            var sqlFlagListOptionBehaviour = (SqlFlagListOptionBehaviour)actual.Options[0].Behaviour;

            Assert.That(sqlFlagListOptionBehaviour.Query, Is.EqualTo(query));
            Assert.That(sqlFlagListOptionBehaviour.ValueMember, Is.EqualTo(memberValue));
            Assert.That(sqlFlagListOptionBehaviour.DisplayMember, Is.EqualTo(displayValue));
        }

        [Test]
        public void Parse_WhenItPresentSqlMultiListBehaviour_ShouldReturnCorrectBehaviour()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var query = Fake<string>();
            var sorted = Fake<bool>();
            var separator = Fake<string>();
            var memberValue = Fake<string>();
            var displayValue = Fake<string>();

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateSqlMultiListBehaviour(optionValue, query, sorted, separator, memberValue, displayValue));

            var actual = Sut.Parse(CreateReader(document));

            Assert.That(actual.Options[0].Behaviour, Is.TypeOf<SqlMultiListOptionBehaviour>());

            var sqlMultiListOptionBehaviour = (SqlMultiListOptionBehaviour)actual.Options[0].Behaviour;

            Assert.That(sqlMultiListOptionBehaviour.Query, Is.EqualTo(query));
            Assert.That(sqlMultiListOptionBehaviour.Sorted, Is.EqualTo(sorted));
            Assert.That(sqlMultiListOptionBehaviour.Separator, Is.EqualTo(separator));
            Assert.That(sqlMultiListOptionBehaviour.ValueMember, Is.EqualTo(memberValue));
            Assert.That(sqlMultiListOptionBehaviour.DisplayMember, Is.EqualTo(displayValue));
        }

        private OptionSet GetExpectedOptionSet(Option actual)
        {
            var optionSet = new OptionSet();
            optionSet.Version = "1";

            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);

            optionSet.Options.Add(new Option
                {
                    Name = actual.Name,
                    DisplayName = OptionAttributeDefaults.DisplayName,
                    Description = OptionAttributeDefaults.Description,
                    DefaultValue = OptionAttributeDefaults.DefaultValue,
                    ValueType = OptionAttributeDefaults.ValueType,
                    Behaviour = new SimpleOptionBehaviour(optionValue)
                });

            return optionSet;
        }

        private XmlTextReader CreateReader(string data)
        {
            return new XmlTextReader(new StringReader(data));
        }

        private XmlTextReader CreateReader(XDocument document)
        {
            var stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            return new XmlTextReader(stream);
        }

        private XDocument GenerateDocumentWithOneOption(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            return GenerateDocument(GenerateOptionFunc(expectedAttribute, name, value, behaviourFunc));
        }

        private Func<IEnumerable<XElement>> GenerateOptionFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            return () => new[] { GenerateOption(expectedAttribute, name, value, behaviourFunc) };
        }

        private XElement GenerateOption(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            var option = new XElement(Keys.Option);

            foreach (var optionAttribute in OptionInformator.Value.OptionAttributes.Where(o => expectedAttribute(o)))
            {
                AddAttribute(option,
                    optionAttribute,
                    name == null || name != optionAttribute.Name
                        ? GetNextValue(optionAttribute)
                        : value);
            }

            if (behaviourFunc != null)
            {
                option.Add(behaviourFunc());
            }

            return option;
        }

        private XDocument GenerateDocument(Func<IEnumerable<XElement>> optionsFunc)
        {
            var declaration = new XDeclaration("1.0", "utf-8", "yes");
            

            var body = new XElement(Keys.OptionSet, optionsFunc());

            return new XDocument(declaration, body);
        }

        private static object GetNextValue(IFixture fixture, XmlSchemaAttribute optionAttribute)
        {
            var type = optionAttribute.AttributeSchemaType.Datatype.ValueType;
            var specimen = new SpecimenContext(fixture).Resolve(type);

            return specimen;
        }

        private object GetNextValue(Type type)
        {
            var specimen = new SpecimenContext(AutoFixture).Resolve(type);

            return specimen;
        }

        private object GetNextValue(XmlSchemaAttribute optionAttribute)
        {
            return GetNextValue(AutoFixture, optionAttribute);
        }

        private void AddAttribute(XElement option, XmlSchemaAttribute optionAttribute, object optionValue)
        {
            option.Add(new XAttribute(optionAttribute.Name, Convert.ToString(optionValue)));
        }

        private static IOptionInformator TraverseXmlSchema(XmlSchema xmlSchema)
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

        private Func<XElement> CreateRangedBehaviourMinMax(IOptionValue optionValue, string minValue, string maxValue, object isMin = null)
        {
            if (isMin == null)
            {
                return () => new XElement("rangedMinMax", new XAttribute("min", optionValue.GetStringValue(minValue)), new XAttribute("max", optionValue.GetStringValue(maxValue)));
            }
            else if ((bool)isMin)
            {
                return () => new XElement("rangedMin", new XAttribute("min", optionValue.GetStringValue(minValue)));
            }
            else if (!(bool)isMin)
            {
                return () => new XElement("rangedMax", new XAttribute("max", optionValue.GetStringValue(maxValue)));               
            }
            else
            {
                return null;
            }
        }

        private Func<XElement> CreateFixedListBehaviour(IOptionValue optionValue, IEnumerable<ListItem> list)
        {
            var fixedList = new XElement("fixedList");

            foreach (var listItem in list)
            {
                fixedList.Add(new XElement("listItem", new XAttribute("value", listItem.Value.ToString()), new XAttribute("displayValue", listItem.DisplayValue)));
            }           

            return () => fixedList;
        }

        private Func<XElement> CreateFlagListBehaviour(IOptionValue optionValue, IEnumerable<ListItem> list)
        {
            var flagList = new XElement("flagList");

            foreach (var listItem in list)
            {
                flagList.Add(new XElement("listItem", new XAttribute("value", listItem.Value.ToString()), new XAttribute("displayValue", listItem.DisplayValue)));
            }

            return () => flagList;
        }

        private Func<XElement> CreateMultiListBehaviour(IOptionValue optionValue, IEnumerable<ListItem> list, bool sorted = false, string separator = ";")
        {
            var multiList = new XElement("multiList", new XAttribute("sorted", sorted), new XAttribute("separator", optionValue.GetStringValue(separator)));

            foreach (var listItem in list)
            {
                multiList.Add(new XElement("listItem", new XAttribute("value", listItem.Value.ToString()), new XAttribute("displayValue", listItem.DisplayValue)));
            }

            return () => multiList;
        }

        private Func<XElement> CreateSqlFixedListBehaviour(IOptionValue optionValue, string query, string memberValue, string displayValue)
        {
            return () => new XElement("sqlFixedList", new XAttribute("query", optionValue.GetStringValue(query)), new XAttribute("valueFieldName", optionValue.GetStringValue(memberValue)), new XAttribute("displayValueFieldName", optionValue.GetStringValue(displayValue)));
        }

        private Func<XElement> CreateSqlFlagListBehaviour(IOptionValue optionValue, string query, string memberValue, string displayValue)
        {
            return () => new XElement("sqlFlagList", new XAttribute("query", optionValue.GetStringValue(query)), new XAttribute("valueFieldName", optionValue.GetStringValue(memberValue)), new XAttribute("displayValueFieldName", optionValue.GetStringValue(displayValue)));
        }

        private Func<XElement> CreateSqlMultiListBehaviour(IOptionValue optionValue, string query, bool sorted, string separator, string memberValue, string displayValue)
        {
            return () => new XElement("sqlMultiList", new XAttribute("sorted", sorted), new XAttribute("separator", optionValue.GetStringValue(separator)), new XAttribute("query", optionValue.GetStringValue(query)), new XAttribute("valueFieldName", optionValue.GetStringValue(memberValue)), new XAttribute("displayValueFieldName", optionValue.GetStringValue(displayValue)));
        }
    }
}
