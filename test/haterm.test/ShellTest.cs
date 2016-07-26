using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace haterm.test
{

    [TestClass]
    public class ShellTest
    {
        private StringRecorder outRecorder = new StringRecorder();
        private StringRecorder errRecorder = new StringRecorder();

        [TestMethod]
        public void StartupTest()
        {
            var shell = new CmdShell(outRecorder, errRecorder);
            errRecorder.List.Should().BeEmpty();
            outRecorder.List.Should().HaveCount(4);
        }
    }
}
