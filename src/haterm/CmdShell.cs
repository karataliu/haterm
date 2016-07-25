using System;
using System.Diagnostics;

namespace haterm
{
    public class CmdShell : IShell, IDisposable
    {
        private Process cmdproc;
        private ProcessThread mainThread;
        private IConsole console;

        public CmdShell(IConsole console)
        {
            this.console = console;
            CmdInit();
        }

        public void Run(string input)
        {
            cmdproc.StandardInput.WriteLine(input);
        }

        public void Dispose()
        {
            this.cmdproc?.CancelOutputRead();
            this.cmdproc?.Kill();
        }

        private void CmdInit()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            cmdproc = Process.Start(startInfo);

            foreach (ProcessThread thread in cmdproc.Threads)
            {
                mainThread = thread;
                break;
            }

            this.cmdproc.OutputDataReceived += Cmdproc_OutputDataReceived;
            this.cmdproc.BeginOutputReadLine();
        }

        private void Cmdproc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            console.WriteLine(e.Data);
        }
    }
}
