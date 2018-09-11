using System;
using NUnit.Framework;

namespace SetMeta.Tests
{
    [Obsolete]
    public static class AssertEx
    {
        [Obsolete]
        public static void ThrowsArgumentNullException(TestDelegate testDelegate, string paramName)
        {
            var ex = Assert.Throws<ArgumentNullException>(testDelegate);

            Assert.That(ex.ParamName, Is.EqualTo(paramName));
        }
    }
}