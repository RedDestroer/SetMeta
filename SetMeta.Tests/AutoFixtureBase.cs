using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using NUnit.Framework;
using SetMeta.Tests.TestDataCreators;
using SetMeta.Tests.TestDataCreators.Abstract;

namespace SetMeta.Tests
{
    public abstract class AutoFixtureBase
    {
        public IFixture AutoFixture { get; private set; }
        public ISetMetaTestDataCreator TestDataCreator { get; private set; }

        [SetUp]
        public void SetUp()
        {
            AutoFixture = new Fixture();
            TestDataCreator = new SetMetaTestDataCreator();
            SetUpInner();
        }

        [TearDown]
        public void TearDown()
        {
            TearDownInner();
        }

        public T Dep<T>()
        {
            return AutoFixture.Freeze<T>();
        }

        public string Fake(string prefix = "RandomString")
        {
            return $"{prefix}{Fake<int>()}";
        }

        public T Fake<T>()
        {
            return AutoFixture.Create<T>();
        }

        public T Fake<T>(Type type)
        {
            return (T)Fake(type);
        }

        public object Fake(Type type)
        {
            return new SpecimenContext(AutoFixture).Resolve(type);
        }

        public T Fake<T>(Func<ICustomizationComposer<T>, IPostprocessComposer<T>> builder)
        {
            return builder(AutoFixture.Build<T>()).Create();
        }

        public IEnumerable<T> FakeMany<T>(Func<ICustomizationComposer<T>, IPostprocessComposer<T>> builder)
        {
            return builder(AutoFixture.Build<T>()).CreateMany();
        }

        public IEnumerable<T> FakeMany<T>()
        {
            return AutoFixture.CreateMany<T>();
        }

        protected virtual void SetUpInner()
        {
        }

        protected virtual void TearDownInner()
        {
        }
    }
}