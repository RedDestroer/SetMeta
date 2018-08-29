using System;
using System.Linq;
using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;

namespace SetMeta.Tests.Util
{
    public static class ArgumentValidationExtensions
    {
        public static T ThrowIfNull<T>(this T o, string paramName) where T : class
        {
            if (o == null)
                throw new ArgumentNullException(paramName);

            return o;
        }

        public static void ShouldNotAcceptNullConstructorArguments(this Type type, IFixture fixture, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var assertion = new GuardClauseAssertion(fixture);
            var constructors = type.GetConstructors(bindingFlags);
            if (!constructors.Any())
                throw new InvalidOperationException($"Constructors for type {type.FullName} isn't found.");

            fixture.Customize(new AutoMoqCustomization());

            assertion.Verify(constructors);
        }
    }
}