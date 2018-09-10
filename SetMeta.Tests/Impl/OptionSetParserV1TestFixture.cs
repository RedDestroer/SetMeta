﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Entities.Suggestions;
using SetMeta.Impl;
using SetMeta.Tests.TestDataCreators;
using SetMeta.Tests.Util;
using SetMeta.Util;
using XsdIterator;
using OptionSetElement = SetMeta.Entities.OptionSetElement;
using OptionElement = SetMeta.Entities.OptionSetElement.OptionElement;
using ListItemElement = SetMeta.Entities.OptionSetElement.OptionElement.FixedListElement.ListItemElement;
using SqlFixedListElement = SetMeta.Entities.OptionSetElement.OptionElement.SqlFixedListElement;
using MultiListElement = SetMeta.Entities.OptionSetElement.OptionElement.MultiListElement;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    internal class OptionSetParserV1TestFixture
        : SutBase<OptionSetParserV1, OptionSetParser>
    {
        private static readonly Lazy<IOptionInformant> OptionInformant;
        private IOptionValueFactory _optionValueFactory = new OptionValueFactory();
        private static readonly IOptionSetValidator ThrowOptionSetValidator = new ExceptionOptionSetValidator();

        static OptionSetParserV1TestFixture()
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
            AutoFixture.Register<IOptionValueFactory>(() => new OptionValueFactory());
            _optionValueFactory = AutoFixture.Create<IOptionValueFactory>();
            base.SetUpInner();
        }

        [Test]
        public void ShouldNotAcceptNullArgumentsForAllConstructors()
        {
            ShouldNotAcceptNullArgumentsForAllConstructorsInner();
        }

        [Test]
        public void ShouldNotAcceptNullArgumentsForAllMethods()
        {
            ShouldNotAcceptNullArgumentsForAllMethodsInner();
        }

        [Test]
        public void Parse_ShouldThrowXmlException_WhenXmlHasNoBody()
        {
            Assert.Throws<XmlException>(() =>
            {
                using (var reader = CreateReader("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"))
                {
                    Sut.Parse(reader, Fake<IOptionSetValidator>());
                }
            });
        }

        [Test]
        public void Parse_ShouldReturnOptionSet_WhenDocumentIsOnlyBody()
        {
            var optionSet = TestDataCreator.OptionSet.Build();

            var actual = Sut.Parse(CreateReader(optionSet), Fake<IOptionSetValidator>());

            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Options, Is.Empty);
        }

        [Test]
        public void Parse_ShouldReturnExpectedOption_WhenOptionsIsSet()
        {
            var option = TestDataCreator.Option.Build(Fake<string>());
            var optionSet = TestDataCreator.OptionSet
                .WithElement(option)
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Options.Count, Is.EqualTo(1));
        }

        [Test]
        public void Parse_ShouldReadOptionAttributes()
        {
            var name = Fake("_");
            var displayName = Fake<string>();
            var description = Fake<string>();
            var defaultValue = Fake<string>();
            var valueType = Fake<OptionValueType>();
            var optionSet = CreateOptionSetWithOneOption(name, tdc => tdc.WithDescription(description)
                .WithDisplayName(displayName)
                .WithDefaultValue(defaultValue)
                .WithValueType(valueType));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Options[name].Name, Is.EqualTo(name));
            Assert.That(actual.Options[name].DisplayName, Is.EqualTo(displayName));
            Assert.That(actual.Options[name].Description, Is.EqualTo(description));
            Assert.That(actual.Options[name].DefaultValue, Is.EqualTo(defaultValue));
            Assert.That(actual.Options[name].ValueType, Is.EqualTo(valueType));
        }

        [Test]
        public void Parse_ShouldReturnDefaults_WhenAttributesAreAbsent()
        {
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Options[name].Name, Is.EqualTo(name));
            Assert.That(actual.Options[name].DisplayName, Is.EqualTo(OptionElement.Attrs.Defaults.DisplayName));
            Assert.That(actual.Options[name].Description, Is.EqualTo(OptionElement.Attrs.Defaults.Description));
            Assert.That(actual.Options[name].DefaultValue, Is.EqualTo(OptionElement.Attrs.Defaults.DefaultValue));
            Assert.That(actual.Options[name].ValueType, Is.EqualTo(OptionElement.Attrs.Defaults.ValueType));
        }

        [Test]
        public void Parse_ShouldReturnRangedOptionBehaviour_WhenOptionContainsRangedMinMaxElement()
        {
            var minValue = Fake<string>();
            var maxValue = Fake<string>();
            var behavior = TestDataCreator.RangedMinMaxBehaviour.Build(minValue, maxValue);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<RangedOptionBehaviour>());

            var actualBehavior = (RangedOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.MaxValue, Is.EqualTo(maxValue));
            Assert.That(actualBehavior.MinValue, Is.EqualTo(minValue));
            Assert.That(actualBehavior.IsMaxValueExists, Is.True);
            Assert.That(actualBehavior.IsMinValueExists, Is.True);
        }

        [Test]
        public void Parse_ShouldReturnRangedOptionBehaviour_WhenOptionContainsRangedMaxElement()
        {
            var maxValue = Fake<string>();
            var behavior = TestDataCreator.RangedMaxBehaviour.Build(maxValue);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<RangedOptionBehaviour>());

            var actualBehavior = (RangedOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.MaxValue, Is.EqualTo(maxValue));
            Assert.That(actualBehavior.IsMaxValueExists, Is.True);
            Assert.That(actualBehavior.IsMinValueExists, Is.False);
        }

        [Test]
        public void Parse_ShouldReturnRangedOptionBehaviour_WhenOptionContainsRangedMinElement()
        {
            var minValue = Fake<string>();
            var behavior = TestDataCreator.RangedMinBehaviour.Build(minValue);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<RangedOptionBehaviour>());

            var actualBehavior = (RangedOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.MinValue, Is.EqualTo(minValue));
            Assert.That(actualBehavior.IsMaxValueExists, Is.False);
            Assert.That(actualBehavior.IsMinValueExists, Is.True);
        }

        [Test]
        public void Parse_ShouldReturnFixedListOptionBehaviour_WhenOptionContainsFixedListElement()
        {
            var listItems = TestDataCreator.ListItemTestDataCreator
                .BuildMany(this)
                .ToList();
            var expectedListItems = CreateExpectedListItems(listItems).ToList();
            var behavior = TestDataCreator.FixedListBehaviour
                .WithListItems(listItems)
                .Build();
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<FixedListOptionBehaviour>());

            var actualBehavior = (FixedListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.ListItems, Is.Not.Null);
            Assert.That(actualBehavior.ListItems.Count, Is.EqualTo(3));
            Assert.That(actualBehavior.ListItems, Is.EquivalentTo(expectedListItems));
        }

        [Test]
        public void Parse_ShouldReturnFixedListOptionBehaviourWithDefaults_WhenOptionContainsFixedListElementWithoutOptionalAttributes()
        {
            var behavior = TestDataCreator.FixedListBehaviour
                .Build();
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<FixedListOptionBehaviour>());

            var actualBehavior = (FixedListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.ListItems, Is.Not.Null);
            Assert.That(actualBehavior.ListItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parse_ShouldReturnFlagListOptionBehaviour_WhenOptionContainsFlagListElement()
        {
            var listItems = TestDataCreator.ListItemTestDataCreator
                .BuildMany(this)
                .ToList();
            var expectedListItems = CreateExpectedListItems(listItems).ToList();
            var behavior = TestDataCreator.FlagListBehaviour
                .WithListItems(listItems)
                .Build();
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<FlagListOptionBehaviour>());

            var actualBehavior = (FlagListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.ListItems, Is.Not.Null);
            Assert.That(actualBehavior.ListItems.Count, Is.EqualTo(3));
            Assert.That(actualBehavior.ListItems, Is.EquivalentTo(expectedListItems));
        }

        [Test]
        public void Parse_ShouldReturnFlagListOptionBehaviourWithDefaults_WhenOptionContainsFlagListElementWithoutOptionalAttributes()
        {
            var behavior = TestDataCreator.FlagListBehaviour
                .Build();
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<FlagListOptionBehaviour>());

            var actualBehavior = (FlagListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.ListItems, Is.Not.Null);
            Assert.That(actualBehavior.ListItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parse_ShouldReturnMultiListOptionBehaviour_WhenOptionContainsMultiListElement()
        {
            var sorted = Fake<bool>();
            var separator = Fake<string>();
            var listItems = TestDataCreator.ListItemTestDataCreator
                .BuildMany(this)
                .ToList();
            var expectedListItems = CreateExpectedListItems(listItems).ToList();
            var behavior = TestDataCreator.MultiListBehaviour
                .WithListItems(listItems)
                .AsSorted(sorted)
                .WithSeparator(separator)
                .Build();
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<MultiListOptionBehaviour>());

            var actualBehavior = (MultiListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.ListItems, Is.Not.Null);
            Assert.That(actualBehavior.ListItems.Count, Is.EqualTo(3));
            Assert.That(actualBehavior.ListItems, Is.EquivalentTo(expectedListItems));
            Assert.That(actualBehavior.Sorted, Is.EqualTo(sorted));
            Assert.That(actualBehavior.Separator, Is.EqualTo(separator));
        }

        [Test]
        public void Parse_ShouldReturnMultiListOptionBehaviourWithDefaults_WhenOptionContainsMultiListElementWithoutOptionalAttributes()
        {
            var behavior = TestDataCreator.MultiListBehaviour
                .Build();
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<MultiListOptionBehaviour>());

            var actualBehavior = (MultiListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.ListItems, Is.Not.Null);
            Assert.That(actualBehavior.ListItems.Count, Is.EqualTo(0));
            Assert.That(actualBehavior.Sorted, Is.EqualTo(MultiListElement.Attrs.Defaults.Sorted));
            Assert.That(actualBehavior.Separator, Is.EqualTo(MultiListElement.Attrs.Defaults.Separator));
        }
        
        [Test]
        public void Parse_ShouldReturnSqlFixedListOptionBehaviour_WhenOptionContainsSqlFixedListElement()
        {
            var query = Fake<string>();
            var valueFieldName = Fake<string>();
            var displayValueFieldName = Fake<string>();
            var behavior = TestDataCreator.SqlFixedListBehaviour
                .WithValueFieldName(valueFieldName)
                .WithDisplayValueFieldName(displayValueFieldName)
                .Build(query);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<SqlFixedListOptionBehaviour>());

            var actualBehavior = (SqlFixedListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.Query, Is.EqualTo(query));
            Assert.That(actualBehavior.ValueMember, Is.EqualTo(valueFieldName));
            Assert.That(actualBehavior.DisplayMember, Is.EqualTo(displayValueFieldName));
        }

        [Test]
        public void Parse_ShouldReturnSqlFixedListOptionBehaviourWithDefaults_WhenOptionContainsSqlFixedListElementWithoutOptionalAttributes()
        {
            var query = Fake<string>();
            var behavior = TestDataCreator.SqlFixedListBehaviour
                .Build(query);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<SqlFixedListOptionBehaviour>());

            var actualBehavior = (SqlFixedListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.Query, Is.EqualTo(query));
            Assert.That(actualBehavior.ValueMember, Is.EqualTo(SqlFixedListElement.Attrs.Defaults.ValueFieldName));
            Assert.That(actualBehavior.DisplayMember, Is.EqualTo(SqlFixedListElement.Attrs.Defaults.DisplayValueFieldName));
        }

        /**Остановился тут**/

        [Test]
        public void Parse_WhenItPresentSqlFlagListBehaviour_ShouldReturnCorrectBehaviour()
        {
            var query = Fake<string>();
            var memberValue = Fake<string>();
            var displayValue = Fake<string>();

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateSqlFlagListBehaviour(query, memberValue, displayValue));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Options.First().Value.Behaviour, Is.TypeOf<SqlFlagListOptionBehaviour>());

            var sqlFlagListOptionBehaviour = (SqlFlagListOptionBehaviour)actual.Options.First().Value.Behaviour;

            Assert.That(sqlFlagListOptionBehaviour.Query, Is.EqualTo(query));
            Assert.That(sqlFlagListOptionBehaviour.ValueMember, Is.EqualTo(memberValue));
            Assert.That(sqlFlagListOptionBehaviour.DisplayMember, Is.EqualTo(displayValue));
        }

        [Test]
        public void Parse_WhenItPresentSqlMultiListBehaviour_ShouldReturnCorrectBehaviour()
        {
            var query = Fake<string>();
            var sorted = Fake<bool>();
            var separator = Fake<string>();
            var memberValue = Fake<string>();
            var displayValue = Fake<string>();

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required, null, null, CreateSqlMultiListBehaviour( query, sorted, separator, memberValue, displayValue));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Options.First().Value.Behaviour, Is.TypeOf<SqlMultiListOptionBehaviour>());

            var sqlMultiListOptionBehaviour = (SqlMultiListOptionBehaviour)actual.Options.First().Value.Behaviour;

            Assert.That(sqlMultiListOptionBehaviour.Query, Is.EqualTo(query));
            Assert.That(sqlMultiListOptionBehaviour.Sorted, Is.EqualTo(sorted));
            Assert.That(sqlMultiListOptionBehaviour.Separator, Is.EqualTo(separator));
            Assert.That(sqlMultiListOptionBehaviour.ValueMember, Is.EqualTo(memberValue));
            Assert.That(sqlMultiListOptionBehaviour.DisplayMember, Is.EqualTo(displayValue));
        }

        [Test]
        public void Parse_WhenItPresentDifferentOptionValueType_ShouldReturnCorrectRealTypes([Values]OptionValueType optionValueType)
        {
            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required || a.Name == "valueType", "valueType", optionValueType.ToString());

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Options.First().Value.Behaviour, Is.TypeOf<SimpleOptionBehaviour>());

            var simpleOptionBehaviour = (SimpleOptionBehaviour)actual.Options.First().Value.Behaviour;

            Assert.That(simpleOptionBehaviour.OptionValueType, Is.EqualTo(optionValueType));

        }

        [Test]
        public void Parse_WhenWePassNotUniqueOptionName_OptionSetValidatorLogError()
        {
            var attributeValue = Fake<string>();
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{OptionSetParser.CreateId(attributeValue)}' isn`t unique among options.";

            var document = GenerateDocumentWithTwoOptionsAndSameNames(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, attributeValue);

            Sut.Parse(CreateReader(document), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_WhenWePassInvalidOptionName_OptionSetValidatorLogError()
        {
            var attributeValue = "#123";
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{OptionSetParser.CreateId(attributeValue)}' ('{attributeValue}') isn`t valid.";

            var document = GenerateDocumentWithOneOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, attributeValue);

            Sut.Parse(CreateReader(document), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_WhenWePassNotUniqueGroupName_OptionSetValidatorLogError()
        {
            var attributeValue = Fake<string>();
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{OptionSetParser.CreateId(attributeValue)}' isn`t unique among groups.";

            var document = GenerateDocumentWithTwoGroupsAndSameNames(a => a.Use == XmlSchemaUse.Required || a.Name == GroupAttributeKeys.Name, GroupAttributeKeys.Name, attributeValue);

            Sut.Parse(CreateReader(document), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_WhenWePassInvalidGroupName_OptionSetValidatorLogError()
        {
            var attributeValue = "#123";
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{OptionSetParser.CreateId(attributeValue)}' ('{attributeValue}') isn`t valid.";

            var document = GenerateDocumentWithOneGroup(a => a.Use == XmlSchemaUse.Required || a.Name == GroupAttributeKeys.Name, GroupAttributeKeys.Name, attributeValue);

            Sut.Parse(CreateReader(document), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
        }

        [TestCase("name", typeof(string), nameof(Group.Name))]
        [TestCase("description", typeof(string), nameof(Group.Description))]
        [TestCase("displayName", typeof(string), nameof(Group.DisplayName))]
        public void Parse_WhenItPresentInGroup_ShouldReadAttribute(string attributeName, Type attributeValueType, string propertyName)
        {
            DataConversion.AddParser(delegate (string input, out object value)
            {
                value = input;
                return true;
            });

            var attributeValue = Fake(attributeValueType);
            var groupAttributeValue = Fake(attributeValueType);

            var document = GenerateDocumentWithOneOptionAndOneGroup(a => a.Use == XmlSchemaUse.Required || a.Name == attributeName, attributeName, attributeValue, groupAttributeValue);

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));

            var propertyInfo = typeof(Group).GetProperty(propertyName);
            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.GetValue(actual.Groups.First().Value), Is.EqualTo(groupAttributeValue));
        }

        [TestCase(GroupAttributeKeys.Description, typeof(string), nameof(Group.Description), GroupAttributeDefaults.Description)]
        [TestCase(GroupAttributeKeys.DisplayName, typeof(string), nameof(Group.DisplayName), GroupAttributeDefaults.DisplayName)]
        public void Parse_WhenItAbsentInGroup_ShouldReturnDefaultValue(string attributeName, Type attributeValueType, string propertyName, object attributeValue)
        {
            DataConversion.AddParser(delegate (string input, out object value)
            {
                value = input;
                return true;
            });

            var document = GenerateDocumentWithOneGroup(a => a.Use == XmlSchemaUse.Required);

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));

            var propertyInfo = typeof(Group).GetProperty(propertyName);
            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.GetValue(actual.Groups.First().Value), Is.EqualTo(attributeValue));
        }

        [Test]
        public void Parse_WhenWePassName_IdIsGeneratedFromName()
        {
            var optionValue = Fake<string>();
            var groupValue = Fake<string>();
            var expectedOptionId = OptionSetParser.CreateId(optionValue);
            var expectedGroupId = OptionSetParser.CreateId(groupValue);

            var document = GenerateDocumentWithOneOptionAndOneGroup(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionValue, groupValue);

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Options.First().Value.Id, Is.EqualTo(expectedOptionId));
            Assert.That(actual.Groups.First().Value.Id, Is.EqualTo(expectedGroupId));
        }

        [Test]
        public void Parse_WhenWePassNotUniqueConstantName_OptionSetValidatorLogError()
        {
            var attributeValue = Fake<string>();
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{OptionSetParser.CreateId(attributeValue)}' isn`t unique among constants.";

            var document = GenerateDocumentWithTwoConstantsAndSameNames(a => a.Use == XmlSchemaUse.Required || a.Name == ConstantAttributeKeys.Name, ConstantAttributeKeys.Name, attributeValue);

            Sut.Parse(CreateReader(document), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_WhenWePassInvalidConstantName_OptionSetValidatorLogError()
        {
            var attributeValue = "#123";
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{OptionSetParser.CreateId(attributeValue)}' ('{attributeValue}') isn`t valid.";

            var document = GenerateDocumentWithOneConstant(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, attributeValue);

            Sut.Parse(CreateReader(document), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
        }

        [TestCase("name", typeof(string), nameof(Constant.Name))]
        [TestCase("valueType", typeof(string), nameof(Constant.ValueType))]
        public void Parse_WhenItPresentInConstant_ShouldReadAttribute(string attributeName, Type attributeValueType, string propertyName)
        {
            DataConversion.AddParser(delegate (string input, out object value)
            {
                value = input;
                return true;
            });

            var attributeValue = Fake(attributeValueType);

            var document = GenerateDocumentWithOneConstant(a => a.Use == XmlSchemaUse.Required || a.Name == attributeName, attributeName, attributeValue);

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Constants, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));

            var propertyInfo = typeof(Constant).GetProperty(propertyName);
            Assert.That(propertyInfo, Is.Not.Null);
            Assert.That(propertyInfo.GetValue(actual.Constants.First().Value), Is.EqualTo(attributeValue));
        }

        [Test]
        public void Parse_WhenItPresentMaxLengthSuggestion_ShouldReturnCorrectSuggestion()
        {
            var max = Fake<ushort>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, 
                groupName, 
                CreateMaxLengthSuggestion(max.ToString()), 
                GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, 
                    optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.MaxLength));
        }

        [Test]
        public void Parse_WhenItPresentMaxLinesSuggestion_ShouldReturnCorrectSuggestion()
        {
            var max = Fake<byte>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, CreateMaxLinesSuggestion(max), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.MaxLines));
        }

        [Test]
        public void Parse_WhenItPresentMinLengthSuggestion_ShouldReturnCorrectSuggestion()
        {
            var min = Fake<UInt16>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, CreateMinLengthSuggestion(min), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.MinLength));
        }

        [Test]
        public void Parse_WhenItPresentMinLinesSuggestion_ShouldReturnCorrectSuggestion()
        {
            var min = Fake<byte>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, CreateMinLinesSuggestion(min), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.MinLines));
        }

        [Test]
        public void Parse_WhenItPresentMultiLineSuggestion_ShouldReturnCorrectSuggestion()
        {
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, CreateMultiLineSuggestion(), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.Multiline));
        }

        [Test]
        public void Parse_WhenItPresentNotifiableSuggestion_ShouldReturnCorrectSuggestion()
        {
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, CreateNotifiableSuggestion(), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.Notifiable));
        }

        [Test]
        public void Parse_WhenItPresentNotifyOnChangeSuggestion_ShouldReturnCorrectSuggestion()
        {
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, CreateNotifyOnChangeSuggestion(), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.NotifyOnChange));
        }

        [Test]
        public void Parse_WhenItPresentRegexSuggestion_ShouldReturnCorrectSuggestion()
        {
            var value = Fake<string>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestion(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, CreateRegexSuggestion(value), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Keys.First(), Is.EqualTo(SuggestionType.Regex));
        }

        [Test]
        public void Parse_WhenWePassNotUniqueSuggestion_OptionSetValidatorLogError()
        {
            var value = Fake<string>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Suggestion with type '{SuggestionType.Regex}' isn`t unique among option '{optionName}'.";

            var document = GenerateDocumentWithOneGroupWithOptionAndTwoSameSuggestions(a => a.Use == XmlSchemaUse.Required || a.Name == ConstantAttributeKeys.Name, ConstantAttributeKeys.Name, groupName, CreateRegexSuggestion(value), CreateRegexSuggestion(value), GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName));

            Sut.Parse(CreateReader(document), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_WhenItPresentConstantWithSuggestionWithThatConstant_ShouldReturnCorrectSuggestion()
        {
            var constantName = "Test";
            var constantValue = Fake<UInt16>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestionAndOneConstant(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName, 
                CreateMaxLengthSuggestion("{Constant name=Test}"), 
                GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName), 
                GenerateConstantWithValue(constantName, constantValue, "UInt16"));

            var actual = Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());

            Assert.That(((MaxLengthSuggestion)actual.Groups.First().Value.Suggestions[OptionSetParser.CreateId(optionName)].Values.First()).Value, Is.EqualTo(constantValue));
        }

        [Test]
        public void Parse_WhenItPresentSuggestionAndThereIsNoConstantWithRightName_Throws()
        {
            var constantName = "Test2";
            var constantValue = Fake<UInt16>();
            var groupName = Fake<string>();
            var optionName = Fake<string>();
            var suggestionValue = "{Constant name=Test}";

            var document = GenerateDocumentWithOneGroupWithOptionAndSuggestionAndOneConstant(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, groupName,
                CreateMaxLengthSuggestion(suggestionValue),
                GenerateOption(a => a.Use == XmlSchemaUse.Required || a.Name == OptionAttributeKeys.Name, OptionAttributeKeys.Name, optionName),
                GenerateConstantWithValue(constantName, constantValue, "UInt16"));

            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                Sut.Parse(CreateReader(document), Fake<IOptionSetValidator>());
            });

            Assert.That(ex.Message, Is.EqualTo($"Невозможно преобразовать значение '{suggestionValue}' к типу '{typeof(UInt16).FullName}'."));
        }

        private List<ListItem> FakeManyListItems(IOptionValue optionValue)
        {
            return FakeMany<ListItem>(o => o.FromFactory(() => new ListItem(Fake(optionValue.ValueType), Fake<string>())))
                .ToList();
        }

        private OptionSet GetExpectedOptionSet(Option actual)
        {
            var optionSet = new OptionSet();
            optionSet.Version = "1";

            var optionValue = _optionValueFactory.Create(actual.ValueType);
            var id = OptionSetParser.CreateId(actual.Name);

            optionSet.Options[id] = new Option
                {
                    Id = id,
                    Name = actual.Name,
                    DisplayName = OptionAttributeDefaults.DisplayName,
                    Description = OptionAttributeDefaults.Description,
                    DefaultValue = OptionAttributeDefaults.DefaultValue,
                    ValueType = OptionAttributeDefaults.ValueType,
                    Behaviour = new SimpleOptionBehaviour(optionValue)
                };

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

        private XDocument GenerateDocumentWithOneGroupWithOptionAndSuggestionAndOneConstant(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value, XElement suggestion, XElement option, XElement constant)
        {
            return GenerateDocument(GenerateOneGroupWithOptionAndSuggestionAndOneConstantFunc(expectedAttribute, name, value, suggestion, option, constant));
        }

        private Func<IEnumerable<XElement>> GenerateOneGroupWithOptionAndSuggestionAndOneConstantFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value, XElement suggestion, XElement option, XElement constant)
        {
            return () => new[] { constant, GenerateGroup(expectedAttribute, name, value, suggestion, option) };
        }

        private XDocument GenerateDocumentWithOneGroupWithOptionAndTwoSameSuggestions(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value, XElement suggestionOne, XElement suggestionTwo, XElement option)
        {
            return GenerateDocument(GenerateOneGroupWithOptionAndTwoSameSuggestionsFunc(expectedAttribute, name, value, suggestionOne, suggestionTwo, option));
        }

        private Func<IEnumerable<XElement>> GenerateOneGroupWithOptionAndTwoSameSuggestionsFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value, XElement suggestionOne, XElement suggestionTwo, XElement option)
        {
            return () => new[] { GenerateGroupWithTowSameSuggestion(expectedAttribute, name, value, suggestionOne, suggestionTwo, option) };
        }

        private XDocument GenerateDocumentWithOneGroupWithOptionAndSuggestion(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value, XElement suggestion, XElement option)
        {
            return GenerateDocument(GenerateOneGroupWithOptionAndSuggestionFunc(expectedAttribute, name, value, suggestion, option));
        }

        private Func<IEnumerable<XElement>> GenerateOneGroupWithOptionAndSuggestionFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value, XElement suggestion, XElement option)
        {
            return () => new[] { GenerateGroup(expectedAttribute, name, value, suggestion, option) };
        }
        
        private XDocument GenerateDocumentWithOneConstant(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value)
        {
            return GenerateDocument(GenerateConstantFunc(expectedAttribute, name, value));
        }

        private Func<IEnumerable<XElement>> GenerateConstantFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value)
        {
            return () => new[] { GenerateConstant(expectedAttribute, name, value) };
        }

        private XDocument GenerateDocumentWithTwoConstantsAndSameNames(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value)
        {
            return GenerateDocument(GenerateOptionFuncWithTwoConstantsAndSameNames(expectedAttribute, name, value));
        }

        private Func<IEnumerable<XElement>> GenerateOptionFuncWithTwoConstantsAndSameNames(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value)
        {
            return () => new[] { GenerateConstant(expectedAttribute, name, value), GenerateConstant(expectedAttribute, name, value) };
        }

        private XDocument GenerateDocumentWithOneOptionAndOneGroup(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object optionValue = null, object groupValue = null)
        {
            return GenerateDocument(GenerateGroupAndOptionFunc(expectedAttribute, name, optionValue, groupValue));
        }

        private Func<IEnumerable<XElement>> GenerateGroupAndOptionFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object optionValue = null, object groupValue = null)
        {
            return () => new[] { GenerateOption(expectedAttribute, name, optionValue), GenerateGroup(expectedAttribute, name, groupValue) };
        }

        private XDocument GenerateDocumentWithTwoGroupsAndSameNames(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null)
        {
            return GenerateDocument(GenerateOptionFuncWithTwoGroupsAndSameNames(expectedAttribute, name, value));
        }

        private Func<IEnumerable<XElement>> GenerateOptionFuncWithTwoGroupsAndSameNames(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null)
        {
            return () => new[] { GenerateGroup(expectedAttribute, name, value), GenerateGroup(expectedAttribute, name, value) };
        }

        private XDocument GenerateDocumentWithOneGroup(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null)
        {
            return GenerateDocument(GenerateGroupFunc(expectedAttribute, name, value));
        }

        private Func<IEnumerable<XElement>> GenerateGroupFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null)
        {
            return () => new[] { GenerateGroup(expectedAttribute, name, value) };
        }

        private XDocument GenerateDocumentWithTwoOptionsAndSameNames(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            return GenerateDocument(GenerateOptionFuncWithTwoOptionsAndSameNames(expectedAttribute, name, value, behaviourFunc));
        }

        private Func<IEnumerable<XElement>> GenerateOptionFuncWithTwoOptionsAndSameNames(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            return () => new[] { GenerateOption(expectedAttribute, name, value, behaviourFunc), GenerateOption(expectedAttribute, name, value, behaviourFunc) };
        }

        private XDocument GenerateDocumentWithOneOption(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            return GenerateDocument(GenerateOptionFunc(expectedAttribute, name, value, behaviourFunc));
        }

        private Func<IEnumerable<XElement>> GenerateOptionFunc(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            return () => new[] { GenerateOption(expectedAttribute, name, value, behaviourFunc) };
        }

        private XElement GenerateConstantWithValue( object nameValue, object value, object valueTypeValue)
        {
            return new XElement(Keys.Constant, new XAttribute("name", nameValue), new XAttribute("value", value), new XAttribute("valueType", valueTypeValue));        
        }

        private XElement GenerateConstant(Predicate<XmlSchemaAttribute> expectedAttribute, string name, object value)
        {
            var constant = new XElement(Keys.Constant);

            foreach (var optionAttribute in OptionInformant.Value.OptionAttributes.Where(o => expectedAttribute(o)))
            {
                AddAttribute(constant,
                    optionAttribute,
                    name == null || name != optionAttribute.Name
                        ? Fake(optionAttribute.AttributeSchemaType.Datatype.ValueType)
                        : value);
            }

            return constant;
        }

        private XElement GenerateOption(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, Func<XElement> behaviourFunc = null)
        {
            var option = new XElement(Keys.Option);

            foreach (var optionAttribute in OptionInformant.Value.OptionAttributes.Where(o => expectedAttribute(o)))
            {
                AddAttribute(option,
                    optionAttribute,
                    name == null || name != optionAttribute.Name
                        ? Fake(optionAttribute.AttributeSchemaType.Datatype.ValueType)
                        : value);
            }

            if (behaviourFunc != null)
            {
                option.Add(behaviourFunc());
            }

            return option;
        }

        private XElement GenerateGroup(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, XElement suggestion = null, XElement option = null)
        {
            var group = new XElement(Keys.Group);

            foreach (var optionAttribute in OptionInformant.Value.OptionAttributes.Where(o => expectedAttribute(o)))
            {
                AddAttribute(group,
                    optionAttribute,
                    name == null || name != optionAttribute.Name
                        ? Fake(optionAttribute.AttributeSchemaType.Datatype.ValueType)
                        : value);
            }

            if (option != null)
            {
                option.Add(suggestion);
                group.Add(option);
            }

            return group;
        }

        private XElement GenerateGroupWithTowSameSuggestion(Predicate<XmlSchemaAttribute> expectedAttribute, string name = null, object value = null, XElement suggestionOne = null, XElement suggestionTwo = null, XElement option = null)
        {
            var group = new XElement(Keys.Group);

            foreach (var optionAttribute in OptionInformant.Value.OptionAttributes.Where(o => expectedAttribute(o)))
            {
                AddAttribute(group,
                    optionAttribute,
                    name == null || name != optionAttribute.Name
                        ? Fake(optionAttribute.AttributeSchemaType.Datatype.ValueType)
                        : value);
            }

            if (option != null)
            {
                option.Add(suggestionOne);
                option.Add(suggestionTwo);
                group.Add(option);
            }

            return group;
        }

        private XDocument GenerateDocument(Func<IEnumerable<XElement>> optionsFunc)
        {
            var declaration = new XDeclaration("1.0", "utf-8", "yes");
            var body = new XElement(OptionSetElement.ElementName, optionsFunc());

            return new XDocument(declaration, body);
        }

        private void AddAttribute(XElement option, XmlSchemaAttribute optionAttribute, object optionValue)
        {
            option.Add(new XAttribute(optionAttribute.Name, Convert.ToString(optionValue)));
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

        private Func<XElement> CreateRangedBehaviourMinMax(IOptionValue optionValue, string minValue, string maxValue, object isMin = null)
        {
            if (isMin == null)
                return () => new XElement("rangedMinMax", new XAttribute("min", optionValue.GetStringValue(minValue)), new XAttribute("max", optionValue.GetStringValue(maxValue)));

            if ((bool) isMin)
                return () => new XElement("rangedMin", new XAttribute("min", optionValue.GetStringValue(minValue)));

            if (!(bool) isMin)
                return () => new XElement("rangedMax", new XAttribute("max", optionValue.GetStringValue(maxValue)));

            return null;
        }

        private Func<XElement> CreateFixedListBehaviour(IOptionValue optionValue, IEnumerable<ListItem> list)
        {
            var fixedList = new XElement("fixedList");

            foreach (var listItem in list)
            {
                fixedList.Add(new XElement("listItem", new XAttribute("value", optionValue.GetStringValue(listItem.Value)), new XAttribute("displayValue", listItem.DisplayValue)));
            }           

            return () => fixedList;
        }

        private Func<XElement> CreateFlagListBehaviour(IOptionValue optionValue, IEnumerable<ListItem> list)
        {
            var flagList = new XElement("flagList");

            foreach (var listItem in list)
            {
                flagList.Add(new XElement("listItem", new XAttribute("value", optionValue.GetStringValue(listItem.Value)), new XAttribute("displayValue", listItem.DisplayValue)));
            }

            return () => flagList;
        }

        private Func<XElement> CreateMultiListBehaviour(IOptionValue optionValue, IEnumerable<ListItem> list, bool sorted = false, string separator = ";")
        {
            var multiList = new XElement("multiList", new XAttribute("sorted", sorted), new XAttribute("separator", separator));

            foreach (var listItem in list)
            {
                multiList.Add(new XElement("listItem", new XAttribute("value", optionValue.GetStringValue(listItem.Value)), new XAttribute("displayValue", listItem.DisplayValue)));
            }

            return () => multiList;
        }

        private Func<XElement> CreateSqlFixedListBehaviour(string query, string memberValue, string displayValue)
        {
            return () => new XElement("sqlFixedList", 
                new XAttribute("query", query),
                new XAttribute("valueFieldName", memberValue),
                new XAttribute("displayValueFieldName", displayValue));
        }

        private Func<XElement> CreateSqlFlagListBehaviour(string query, string memberValue, string displayValue)
        {
            return () => new XElement("sqlFlagList", 
                new XAttribute("query", query),
                new XAttribute("valueFieldName", memberValue),
                new XAttribute("displayValueFieldName", displayValue));
        }

        private Func<XElement> CreateSqlMultiListBehaviour(string query, bool sorted, string separator, string memberValue, string displayValue)
        {
            return () => new XElement("sqlMultiList", 
                new XAttribute("sorted", sorted), 
                new XAttribute("separator", separator), 
                new XAttribute("query", query), 
                new XAttribute("valueFieldName", memberValue), 
                new XAttribute("displayValueFieldName", displayValue));
        }

        private XElement CreateMaxLengthSuggestion(string max)
        {
            return new XElement("suggestion",new XElement("maxLength", new XAttribute("value", max)));
        }

        private XElement CreateMaxLinesSuggestion(byte max)
        {
            return new XElement("suggestion", new XElement("maxLines", new XAttribute("value", max)));
        }

        private XElement CreateMinLengthSuggestion(ushort min)
        {
            return new XElement("suggestion", new XElement("minLength", new XAttribute("value", min)));
        }

        private XElement CreateMinLinesSuggestion(byte min)
        {
            return new XElement("suggestion", new XElement("minLines", new XAttribute("value", min)));
        }

        private XElement CreateMultiLineSuggestion()
        {
            return new XElement("suggestion", new XElement("multiline"));
        }

        private XElement CreateNotifiableSuggestion()
        {
            return new XElement("suggestion", new XElement("notifiable"));
        }

        private XElement CreateNotifyOnChangeSuggestion()
        {
            return new XElement("suggestion", new XElement("notifyOnChange"));
        }

        private XElement CreateRegexSuggestion(string value)
        {
            return new XElement("suggestion", new XElement("regex", new XAttribute("value", value)));
        }

        private XDocument CreateOptionSetWithOneOption(string name, Func<IOptionTestDataCreator, IOptionTestDataCreator> augment = null)
        {
            var tdc = TestDataCreator.Option;
            if (augment != null)
                tdc = augment(tdc);
            var option = tdc.Build(name);
            var optionSet = TestDataCreator.OptionSet
                .WithElement(option)
                .Build();

            return optionSet;
        }

        private (string, XDocument) CreateOptionSetWithOneOption(Func<IOptionTestDataCreator, IOptionTestDataCreator> augment = null)
        {
            var name = Fake("_");
            var tdc = TestDataCreator.Option;
            if (augment != null)
                tdc = augment(tdc);
            var option = tdc.Build(name);
            var optionSet = TestDataCreator.OptionSet
                .WithElement(option)
                .Build();

            return (name, optionSet);
        }

        private IEnumerable<ListItem> CreateExpectedListItems(IEnumerable<XElement> elements)
        {
            return elements.Select(o => new ListItem((object)o.GetAttributeValue<string>(ListItemElement.Attrs.Value), o.GetAttributeValue<string>(ListItemElement.Attrs.DisplayValue)));
        }

        private class ExceptionOptionSetValidator
            : IOptionSetValidator
        {
            public void AddError(string message, IXmlLineInfo xmlLineInfo = null)
            {
                throw new AssertionException("Validator should not be called.");
            }
        }
    }
}
