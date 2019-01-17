using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SetMeta.Abstract;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;
using SetMeta.Tests.TestDataCreators.Abstract;
using SetMeta.Tests.TestDataCreators.Extensions;
using SetMeta.Util;
using OptionElement = SetMeta.XmlKeys.OptionSetElement.OptionElement;
using ListItemElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.FixedListElement.ListItemElement;
using SqlFixedListElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.SqlFixedListElement;
using MultiListElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.MultiListElement;
using SqlFlagListElement = SetMeta.XmlKeys.OptionSetElement.OptionElement.SqlFlagListElement;
using SqlMultiListElement= SetMeta.XmlKeys.OptionSetElement.OptionElement.SqlMultiListElement;
using ConstantElement = SetMeta.XmlKeys.OptionSetElement.ConstantElement;
using SuggestionElement = SetMeta.XmlKeys.OptionSetElement.SuggestionElement;

namespace SetMeta.Tests.Impl
{
    [TestFixture]
    internal class OptionSetParserV1TestFixture
        : SutBase<OptionSetParserV1, OptionSetParser>
    {
        private static readonly IOptionSetValidator ThrowOptionSetValidator = new ExceptionOptionSetValidator();

        protected override void SetUpInner()
        {
            AutoFixture.Register<IOptionValueFactory>(() => new OptionValueFactory());
            base.SetUpInner();
            Sut.IdFactory = new DefaultIdFactory();
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
        public void IdFactory_ShouldBeInitialized()
        {
            var optionValueFactory = Fake<IOptionValueFactory>();
            var target = new OptionSetParserV1(optionValueFactory);
            Assert.That(target.IdFactory, Is.Not.Null);
            Assert.That(target.IdFactory, Is.TypeOf<DefaultIdFactory>());
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
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Options, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Options.Count, Is.EqualTo(1));
            Assert.That(actual.Options.ContainsKey(name), Is.True);
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
        public void Parse_ShouldReturnOptionDefaults_WhenAttributesAreAbsent()
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

        [Test]
        public void Parse_ShouldReturnSqlFlagListOptionBehaviour_WhenOptionContainsSqlFlagListElement()
        {
            var query = Fake<string>();
            var valueFieldName = Fake<string>();
            var displayValueFieldName = Fake<string>();
            var behavior = TestDataCreator.SqlFlagListBehaviour
                .WithValueFieldName(valueFieldName)
                .WithDisplayValueFieldName(displayValueFieldName)
                .Build(query);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<SqlFlagListOptionBehaviour>());

            var actualBehavior = (SqlFlagListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.Query, Is.EqualTo(query));
            Assert.That(actualBehavior.ValueMember, Is.EqualTo(valueFieldName));
            Assert.That(actualBehavior.DisplayMember, Is.EqualTo(displayValueFieldName));
        }

        [Test]
        public void Parse_ShouldReturnSqlFlagListOptionBehaviourWithDefaults_WhenOptionContainsSqlFlagListElementWithoutOptionalAttributes()
        {
            var query = Fake<string>();
            var behavior = TestDataCreator.SqlFlagListBehaviour
                .Build(query);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<SqlFlagListOptionBehaviour>());

            var actualBehavior = (SqlFlagListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.Query, Is.EqualTo(query));
            Assert.That(actualBehavior.ValueMember, Is.EqualTo(SqlFlagListElement.Attrs.Defaults.ValueFieldName));
            Assert.That(actualBehavior.DisplayMember, Is.EqualTo(SqlFlagListElement.Attrs.Defaults.DisplayValueFieldName));
        }

        [Test]
        public void Parse_ShouldReturnSqlMultiListOptionBehaviour_WhenOptionContainsSqlMultiListElement()
        {
            var query = Fake<string>();
            var sorted = Fake<bool>();
            var separator = Fake<string>();
            var valueFieldName = Fake<string>();
            var displayValueFieldName = Fake<string>();
            var behavior = TestDataCreator.SqlMultiListBehaviour
                .WithSeparator(separator)
                .AsSorted(sorted)
                .WithValueFieldName(valueFieldName)
                .WithDisplayValueFieldName(displayValueFieldName)
                .Build(query);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<SqlMultiListOptionBehaviour>());

            var actualBehavior = (SqlMultiListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.Query, Is.EqualTo(query));
            Assert.That(actualBehavior.Sorted, Is.EqualTo(sorted));
            Assert.That(actualBehavior.Separator, Is.EqualTo(separator));
            Assert.That(actualBehavior.ValueMember, Is.EqualTo(valueFieldName));
            Assert.That(actualBehavior.DisplayMember, Is.EqualTo(displayValueFieldName));
        }

        [Test]
        public void Parse_ShouldReturnSqlMultiListOptionBehaviourWithDefaults_WhenOptionContainsSqlMultiListElementWithoutOptionalAttributes()
        {
            var query = Fake<string>();
            var behavior = TestDataCreator.SqlMultiListBehaviour
                .Build(query);
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithBehaviour(behavior));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<SqlMultiListOptionBehaviour>());

            var actualBehavior = (SqlMultiListOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(actualBehavior.Query, Is.EqualTo(query));
            Assert.That(actualBehavior.Sorted, Is.EqualTo(SqlMultiListElement.Attrs.Defaults.Sorted));
            Assert.That(actualBehavior.Separator, Is.EqualTo(SqlMultiListElement.Attrs.Defaults.Separator));
            Assert.That(actualBehavior.ValueMember, Is.EqualTo(SqlMultiListElement.Attrs.Defaults.ValueFieldName));
            Assert.That(actualBehavior.DisplayMember, Is.EqualTo(SqlMultiListElement.Attrs.Defaults.DisplayValueFieldName));
        }

        [Test]
        public void Parse_ShouldReturnCorrectOptionValueType_ForAllOptionValueTypeEnum([Values]OptionValueType optionValueType)
        {
            (string name, XDocument optionSet) = CreateOptionSetWithOneOption(tdc => tdc.WithValueType(optionValueType));

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Options[name].Behaviour, Is.TypeOf<SimpleOptionBehaviour>());

            var simpleOptionBehaviour = (SimpleOptionBehaviour)actual.Options[name].Behaviour;

            Assert.That(simpleOptionBehaviour.OptionValueType, Is.EqualTo(optionValueType));
        }

        [Test]
        public void Parse_ShouldLogError_WhenWePassNotUniqueOptionName()
        {
            var name = Fake("_");
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{Sut.IdFactory.CreateId(name)}' isn`t unique among options.";
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Option.Build(name))
                .WithElement(TestDataCreator.Option.Build(name))
                .Build();

            Sut.Parse(CreateReader(optionSet), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(It.IsAny<string>(), It.IsAny<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_ShouldLogError_WhenWePassInvalidOptionName()
        {
            var name = Fake("#");
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage1 = $"Key '{Sut.IdFactory.CreateId(name)}' ('{name}') isn`t valid.";
            var expectedMessage2 = $"Name '{name}' isn`t valid.";
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Option.Build(name))
                .Build();

            Sut.Parse(CreateReader(optionSet), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage1, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(expectedMessage2, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(It.IsAny<string>(), It.IsAny<IXmlLineInfo>()), Times.Exactly(2));
        }

        [Test]
        public void Parse_ShouldLogError_WhenWePassNotUniqueGroupName()
        {
            var name = Fake("_");
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{Sut.IdFactory.CreateId(name)}' isn`t unique among groups.";
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Group.Build(name))
                .WithElement(TestDataCreator.Group.Build(name))
                .Build();

            Sut.Parse(CreateReader(optionSet), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(It.IsAny<string>(), It.IsAny<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_ShouldLogError_WhenWePassInvalidGroupName()
        {
            var name = Fake("#");
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage1 = $"Key '{Sut.IdFactory.CreateId(name)}' ('{name}') isn`t valid.";
            var expectedMessage2 = $"Name '{name}' isn`t valid.";
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Group.Build(name))
                .Build();

            Sut.Parse(CreateReader(optionSet), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage1, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(expectedMessage2, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(It.IsAny<string>(), It.IsAny<IXmlLineInfo>()), Times.Exactly(2));
        }

        [Test]
        public void Parse_ShouldLogError_WhenWePassNotUniqueConstantName()
        {
            var name = Fake("_");
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Key '{Sut.IdFactory.CreateId(name)}' isn`t unique among constants.";
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Constant.Build(name))
                .WithElement(TestDataCreator.Constant.Build(name))
                .Build();

            Sut.Parse(CreateReader(optionSet), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(It.IsAny<string>(), It.IsAny<IXmlLineInfo>()), Times.Once);
        }

        [Test]
        public void Parse_ShouldLogError_WhenWePassInvalidConstantName()
        {
            var name = Fake("#");
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage1 = $"Key '{Sut.IdFactory.CreateId(name)}' ('{name}') isn`t valid.";
            var expectedMessage2 = $"Name '{name}' isn`t valid.";
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Constant.Build(name))
                .Build();

            Sut.Parse(CreateReader(optionSet), mock.Object);

            mock.Verify(o => o.AddError(expectedMessage1, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(expectedMessage2, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(It.IsAny<string>(), It.IsAny<IXmlLineInfo>()), Times.Exactly(2));
        }

        [Test]
        public void Parse_ShouldGenerateIdFromNameOfOption()
        {
            var name = Fake("_");
            var expectedId = Sut.IdFactory.CreateId(name);
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Option.Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Options[name].Id, Is.EqualTo(expectedId));
        }

        [Test]
        public void Parse_ShouldGenerateIdFromNameOfGroup()
        {
            var name = Fake("_");
            var expectedId = Sut.IdFactory.CreateId(name);
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Group.Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Groups[name].Id, Is.EqualTo(expectedId));
        }

         [Test]
        public void Parse_ShouldReadConstantAttributes()
        {
            var name = Fake("_");
            var value = Fake<string>();
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator
                    .Constant
                    .WithValue(value)
                    .Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Constants, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Constants[name].Name, Is.EqualTo(name));
            Assert.That(actual.Constants[name].Value, Is.EqualTo(value));
        }

        [Test]
        public void Parse_ShouldReturnConstantDefaults_WhenAttributesAreAbsent()
        {
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Constant.Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);
            Assert.That(actual.Constants, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Constants[name].Name, Is.EqualTo(name));
            Assert.That(actual.Constants[name].Value, Is.EqualTo(ConstantElement.Attrs.Defaults.Value));
        }

        [Test]
        public void Parse_ShouldReturnCorrectSuggestion_WhenItPresentMaxLengthSuggestion()
        {
            var max = Fake<int>();
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithMaxLength(max.ToString()).Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.MaxLength), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MaxLength].Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MaxLength].ContainsKey(SuggestionElement.MaxLengthElement.Attrs.Value), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MaxLength][SuggestionElement.MaxLengthElement.Attrs.Value], Is.EqualTo(max.ToString()));
        }

        [Test]
        public void Parse_WhenItPresentMaxLinesSuggestion_ShouldReturnCorrectSuggestion()
        {
            var max = Fake<int>();
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithMaxLines(max.ToString()).Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.MaxLines), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MaxLines].Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MaxLines].ContainsKey(SuggestionElement.MaxLinesElement.Attrs.Value), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MaxLines][SuggestionElement.MaxLinesElement.Attrs.Value], Is.EqualTo(max.ToString()));
        }

        [Test]
        public void Parse_WhenItPresentMinLengthSuggestion_ShouldReturnCorrectSuggestion()
        {
            var min = Fake<int>();
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithMinLength(min.ToString()).Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.MinLength), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MinLength].Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MinLength].ContainsKey(SuggestionElement.MinLengthElement.Attrs.Value), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MinLength][SuggestionElement.MinLengthElement.Attrs.Value], Is.EqualTo(min.ToString()));
        }

        [Test]
        public void Parse_WhenItPresentMinLinesSuggestion_ShouldReturnCorrectSuggestion()
        {
            var min = Fake<int>();
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithMinLines(min.ToString()).Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.MinLines), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MinLines].Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MinLines].ContainsKey(SuggestionElement.MinLinesElement.Attrs.Value), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MinLines][SuggestionElement.MinLinesElement.Attrs.Value], Is.EqualTo(min.ToString()));
        }

        [Test]
        public void Parse_WhenItPresentMultiLineSuggestion_ShouldReturnCorrectSuggestion()
        {
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithMultiline().Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.Multiline), Is.True);
        }

        [Test]
        public void Parse_WhenItPresentNotifiableSuggestion_ShouldReturnCorrectSuggestion()
        {
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithNotifiable().Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.Notifiable), Is.True);
        }

        [Test]
        public void Parse_WhenItPresentNotifyOnChangeSuggestion_ShouldReturnCorrectSuggestion()
        {
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithNotifyOnChange().Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.NotifyOnChange), Is.True);
        }

        [Test]
        public void Parse_WhenItPresentRegexSuggestion_ShouldReturnCorrectSuggestion()
        {
            var value = Fake<string>();
            var validation = Fake<string>();
            var name = Fake("_");
            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Suggestion.WithRegex(value, validation).Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Name, Is.EqualTo(name));
            Assert.That(actual.Suggestions[name].Params, Is.Not.Null);
            Assert.That(actual.Suggestions[name].Params.Count, Is.EqualTo(1));
            Assert.That(actual.Suggestions[name].Params.ContainsKey(SuggestionType.Regex), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.Regex].Count, Is.EqualTo(2));
            Assert.That(actual.Suggestions[name].Params[SuggestionType.Regex].ContainsKey(SuggestionElement.RegexElement.Attrs.Value), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.Regex][SuggestionElement.RegexElement.Attrs.Value], Is.EqualTo(value));
            Assert.That(actual.Suggestions[name].Params[SuggestionType.Regex].ContainsKey(SuggestionElement.RegexElement.Attrs.Validation), Is.True);
            Assert.That(actual.Suggestions[name].Params[SuggestionType.Regex][SuggestionElement.RegexElement.Attrs.Validation], Is.EqualTo(validation));
        }

        [Test]
        public void Parse_WhenItPresentConstantWithSuggestionWithThatConstant_ShouldReturnCorrectSuggestion()
        {
            var name = Fake("_");
            var constantName = "Test";
            var constantValue = Fake<int>();

            var optionSet = TestDataCreator.OptionSet
                .WithElement(TestDataCreator.Constant.WithValue(constantValue.ToString()).Build(constantName))
                .WithElement(TestDataCreator.Suggestion.WithMinLength("{Constant name=Test}").Build(name))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Suggestions[name].Params[SuggestionType.MinLength][SuggestionElement.MinLengthElement.Attrs.Value], Is.EqualTo(actual.Constants[constantName].Value.ToString()));
        }

        [Test]
        public void Parse_WhenItPresentGroupWithSuggestion_SuggestionInGroupIsReplacedWithSuggestionFromOptionSet()
        {
            var max = Fake<int>();
            var suggestionName = Fake("_");
            var optionName = Fake("_");
            var groupName = Fake("_");
            var optionSet = TestDataCreator.OptionSet                
                .WithElement(TestDataCreator.Group.WithElement(TestDataCreator.Option.WithElement(TestDataCreator.OptionSuggestion.Build(suggestionName)).Build(optionName)).Build(groupName))
                .WithElement(TestDataCreator.Suggestion.WithMaxLength(max.ToString()).Build(suggestionName))
                .Build();

            var actual = Sut.Parse(CreateReader(optionSet), ThrowOptionSetValidator);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Groups[groupName].GroupOptions.First().Suggestions[SuggestionType.MaxLength][SuggestionElement.MaxLengthElement.Attrs.Value], Is.EqualTo(actual.Suggestions[suggestionName].Params[SuggestionType.MaxLength][SuggestionElement.MaxLengthElement.Attrs.Value]));
        }

        [Test]
        public void Parse_ShouldReturnValidation_WhenSuggestionInOptionGroupIsNotFoundAmongOptionSetSuggestions()
        {
            var max = Fake<int>();
            var suggestionName = Fake("_");
            var unexpectedSuggestionName = Fake("_");
            var optionName = Fake("_");
            var groupName = Fake("_");
            var mock = Fake<Mock<IOptionSetValidator>>();
            var expectedMessage = $"Suggestion with name '{unexpectedSuggestionName}' isn`t found among optionSet suggestions.";
            var optionSet = TestDataCreator.OptionSet
                                           .WithElement(TestDataCreator.Group
                                               .WithElement(TestDataCreator.Option
                                                   .WithElement(TestDataCreator.OptionSuggestion
                                                       .Build(unexpectedSuggestionName))
                                                   .Build(optionName))
                                               .Build(groupName))
                                           .WithElement(TestDataCreator.Suggestion.WithMaxLength(max.ToString()).Build(suggestionName))
                                           .Build();

            var actual = Sut.Parse(CreateReader(optionSet), mock.Object);

            Assert.That(actual.Suggestions, Is.Not.Null);
            Assert.That(actual.Version, Is.EqualTo("1"));
            Assert.That(actual.Groups[groupName].GroupOptions.First().Suggestions, Is.Empty);
            mock.Verify(o => o.AddError(expectedMessage, It.IsNotNull<IXmlLineInfo>()), Times.Once);
            mock.Verify(o => o.AddError(It.IsAny<string>(), It.IsAny<IXmlLineInfo>()), Times.Once);
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
            return elements.Select(o => new ListItem(o.GetAttributeValue<string>(ListItemElement.Attrs.Value), o.GetAttributeValue<string>(ListItemElement.Attrs.DisplayValue)));
        }

        private class ExceptionOptionSetValidator
            : IOptionSetValidator
        {
            public void AddError(string message, IXmlLineInfo xmlLineInfo = null)
            {
                throw new AssertionException($"Validator should not be called, but was called with: {{ message: '{message}', xmlLineInfo: '{xmlLineInfo}' }}.");
            }
        }
    }
}
