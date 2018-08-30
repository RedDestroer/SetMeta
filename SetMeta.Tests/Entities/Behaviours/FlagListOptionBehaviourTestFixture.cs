using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class FlagListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void FlagListOptionBehaviour_ConstructorNullChecks()
        {
            typeof(FlagListOptionBehaviour).ShouldNotAcceptNullConstructorArguments(AutoFixture, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void FlagListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = Fake<List<ListItem>>();

            var actual = new FlagListOptionBehaviour(optionValue, list);

            Assert.That(actual.ListItems, Is.EqualTo(list));
        }
    }
}