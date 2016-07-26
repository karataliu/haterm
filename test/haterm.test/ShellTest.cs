using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace haterm.test
{
    [TestClass]
    public class ShellTest
    {
        private OutRecorder recorder = new OutRecorder();
        
        [TestMethod]
        public void StartupTest()
        {
            using (var shell = new CmdShell(recorder))
            {
                recorder.Out.Should().HaveCount(5);
                recorder.Out[3].Should().StartWith("Haterm");
                recorder.Out[4].Should().BeEmpty();

                recorder.Err.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void ExitTest()
        {
            var shell = new CmdShell(recorder);
            shell.Exited.Should().BeFalse();
            shell.Run("exit");
            shell.Exited.Should().BeTrue();
        }

        [TestMethod]
        public void RunCmdTest()
        {
            using (var shell = new CmdShell(recorder))
            {
                recorder.Out.Clear();
                recorder.Err.Clear();

                shell.Run("echo 1");
                recorder.Out.Should().HaveCount(2);
                recorder.Err.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void RunErrorCmdTest()
        {
            using (var shell = new CmdShell(recorder))
            {
                recorder.Out.Clear();
                recorder.Err.Clear();

                shell.Run("echo1");
                recorder.Out.Should().HaveCount(1);
                recorder.Out[0].Should().BeEmpty();
                recorder.Err.Should().HaveCount(2);
            }
        }
    }
}
