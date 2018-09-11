using System.Reflection;
using NUnit.Framework;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;
using SetMeta.Tests.Util;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class FixedListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void ShouldNotAcceptNullArgumentsForAllConstructors()
        {
            typeof(FixedListOptionBehaviour).ShouldNotAcceptNullConstructorArguments(AutoFixture, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        [Test]
        public void FixedListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = FakeMany<ListItem>();

            var actual = new FixedListOptionBehaviour(optionValue, list);

            Assert.That(actual.ListItems, Is.EqualTo(list));           
        }
    }
}