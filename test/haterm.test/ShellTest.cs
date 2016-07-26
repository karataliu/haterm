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
            using (var shell = new CmdShell(outRecorder, errRecorder))
            {
                errRecorder.List.Should().BeEmpty();
                outRecorder.List.Should().HaveCount(5);

                outRecorder.List[3].Should().StartWith("Haterm");
                outRecorder.List[4].Should().BeEmpty();
            }
        }

        [TestMethod]
        public void ExitTest()
        {
            var shell = new CmdShell(outRecorder, errRecorder);
            shell.Exited.Should().BeFalse();
            shell.Run("exit");
            shell.Exited.Should().BeTrue();
        }
    }
}
