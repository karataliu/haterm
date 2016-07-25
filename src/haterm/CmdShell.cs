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
            this.cmdproc?.CancelOutputRead();
            this.cmdproc?.Kill();
        }

        private void CmdInit()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                UseShellExecute = false,
                RedirectStandardError = true,
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

            this.cmdproc.OutputDataReceived += (sender, args) =>
            {
                console.WriteLine(args.Data);
            };

            this.cmdproc.BeginOutputReadLine();

            this.cmdproc.ErrorDataReceived += (sender, args) =>
            {
                this.console.Write1(new TextBlock
                {
                    Text = args.Data,
                    Foreground = ConsoleColor.Red
                });
                this.console.WriteLine("");
            };
            this.cmdproc.BeginErrorReadLine();
        }
    }
}
