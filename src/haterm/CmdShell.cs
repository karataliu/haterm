using System;
using System.Diagnostics;
using System.Threading;

namespace haterm
{
    public class CmdShell : IShell, IDisposable
    {
        private const string Rem = "rem";
        private readonly string HaRunId = Guid.NewGuid().ToString();
        private readonly string HaLineEnd;
        private Process cmdproc;
        private IConsole console;
        private ManualResetEventSlim lineEvent = new ManualResetEventSlim(false);

        public string CurrentDir { get; set; }

        public CmdShell(IConsole console)
        {
            this.console = console;
            HaLineEnd = $"{Rem} {HaRunId}{nameof(HaLineEnd)}";

            CmdInit();
        }

        public void Run(string input)
        {
            cmdproc.StandardInput.WriteLine(input);
            UpdateCwd();
        }

        private void UpdateCwd()
        {
            cmdproc.StandardInput.WriteLine(HaLineEnd);
            this.lineEvent.Wait();
            this.lineEvent.Reset();
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

            int skipLine = 0;
            this.cmdproc.OutputDataReceived += (sender, args) =>
            {
                var line = args.Data;
                if (line == null)
                {
                    return;
                }

                if (skipLine > 0)
                {
                    --skipLine;
                    return;
                }

                

                if (line.EndsWith(this.HaLineEnd))
                {
                    this.CurrentDir = line.Substring(0, line.Length - this.HaLineEnd.Length - 1);
                    this.lineEvent.Set();
                    skipLine = 2;
                    return;
                }

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

            this.UpdateCwd();
            // this.console.WriteLine("Ready.");
        }
    }
}
