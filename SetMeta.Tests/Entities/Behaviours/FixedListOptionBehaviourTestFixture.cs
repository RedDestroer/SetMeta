using System.Collections.Generic;
using NUnit.Framework;
using SetMeta.Entities;
using SetMeta.Entities.Behaviours;
using SetMeta.Impl;

namespace SetMeta.Tests.Entities.Behaviours
{
    [TestFixture]
    public class FixedListOptionBehaviourTestFixture
        : AutoFixtureBase
    {
        [Test]
        public void FixedListOptionBehaviour_WhenWePassNull_ThrowException()
        {
            void Delegate()
            {
                new FixedListOptionBehaviour(null, Fake<List<ListItem>>());
            }

            AssertEx.ThrowsArgumentNullException(Delegate, "optionValue");
        }

        [Test]
        public void FixedListOptionBehaviour_WhenWePassValidItems_TheyAssignedCorrectly()
        {
            var optionValueFactory = new OptionValueFactory();
            var optionValue = optionValueFactory.Create(OptionValueType.String);
            var list = Fake<List<ListItem>>();

            var actual = new FixedListOptionBehaviour(optionValue, list);

            Assert.That(actual.ListItems, Is.EqualTo(list));           
        }
    }
}