using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace haterm.test
{
    [TestClass]
    public class HintTest
    {
        [TestMethod]
        public void GetCommonPrefixTest()
        {
            var list = new[]
            {
                "ab123",
                "ad213"
            };

            HatermHint.getCommonPrefix(list).Should().Be("a");


        }

        [TestMethod]
        public void GetCommonPrefixTest2()
        {
            var list = new[]
            {
                "demaster",
                "dev1",
                "dev2",
            };

            HatermHint.getCommonPrefix(list).Should().Be("de");
        }
    }
}
