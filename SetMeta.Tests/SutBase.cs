﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using NUnit.Framework;

namespace SetMeta.Tests
{
    [TestFixture]
    public class SutBase<TSut, TContract>
        : AutoFixtureBase
        where TSut : class, TContract
        where TContract : class
    {
        public TContract Sut => AutoFixture.Freeze<TSut>();
        
        ////public void Chain<TParent, TChild>(Func<TParent, TChild> expression) where TParent : class where TChild : class
        ////{
        ////    Dep<TParent>().Stub(expression).Return(Dep<TChild>());
        ////}


        protected override void SetUpInner()
        {
            base.SetUpInner();
            AutoFixture.Customize(new AutoMoqCustomization());
            AutoFixture.Behaviors.Add(new OmitOnRecursionBehavior());
            AutoFixture.Customize<TSut>(o => o.FromFactory(new MethodInvoker(new GreedyConstructorQuery())));
        }
    }
}