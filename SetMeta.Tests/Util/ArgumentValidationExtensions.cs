using System;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using NUnit.Framework;

namespace SetMeta.Tests.Util
{
    public static class ArgumentValidationExtensions
    {
        public static void ShouldThrowArgumentNullException<T>(this T o, Action<T> action, string argumentName) where T : class
        {
            var ex = Assert.Throws<ArgumentNullException>(() => action(o));

            Assert.That(ex.ParamName, Is.EqualTo(argumentName));
        }

        public static void ShouldNotAcceptNullConstructorArguments(this Type type, IFixture fixture, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var assertion = new GuardClauseAssertion(fixture);
            var constructorInfos = type.GetConstructors(bindingFlags);
            if (!constructorInfos.Any())
                throw new InvalidOperationException($"Constructors for type {type.FullName} isn't found.");

            fixture.Customize(new AutoMoqCustomization());

            assertion.Verify(constructorInfos);
        }

        public static void ShouldNotAcceptNullMethodArguments(this Type type, IFixture fixture, string methodName, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var assertion = new GuardClauseAssertion(fixture);
            var methodInfo = type.GetMethod(methodName, bindingFlags);
            if (methodInfo == null)
                throw new InvalidOperationException($"Method '{methodName}' for type {type.FullName} isn't found.");

            fixture.Customize(new AutoMoqCustomization());

            assertion.Verify(methodInfo);
        }

        public static void ShouldNotAcceptNullArgumentsForAllMethods(this Type type, IFixture fixture, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            type.ShouldNotAcceptNullArgumentsForAllMethods(fixture, mi => true, bindingFlags);
        }

        public static void ShouldNotAcceptNullArgumentsForAllMethods(this Type type, IFixture fixture, Func<MethodInfo, bool> whereFunc, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var assertion = new GuardClauseAssertion(fixture);
            var methodInfos = type.GetMethods(bindingFlags);
            if (!methodInfos.Where(whereFunc).Any())
                throw new InvalidOperationException($"Methods for type {type.FullName} isn't found.");

            fixture.Customize(new AutoMoqCustomization());

            assertion.Verify(methodInfos.Where(whereFunc));
        }
    }
}