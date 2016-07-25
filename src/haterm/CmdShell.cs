using System;
using System.Diagnostics;
using System.Threading;

namespace haterm
{
    public class CmdShell : IShell, IDisposable
    {
        private const string Rem = "rem";
        private readonly string HaRunId = Guid.NewGuid().ToString();
        private readonly string HaStart;
        private readonly string HaLineStart;
        private readonly string HaLineEnd;
        private Process cmdproc;
        private IConsole console;
        private ManualResetEventSlim startEvent = new ManualResetEventSlim(false);
        private ManualResetEventSlim lineEvent = new ManualResetEventSlim(false);

        public string CurrentDir { get; set; }

        public CmdShell(IConsole console)
        {
            this.console = console;
            HaStart = $"{Rem} {HaRunId}{nameof(HaStart)}";
            HaLineStart = $"{Rem} {HaRunId}{nameof(HaLineStart)}";
            HaLineEnd = $"{Rem} {HaRunId}{nameof(HaLineEnd)}";

            CmdInit();
        }

        public void Run(string input)
        {
            cmdproc.StandardInput.WriteLine(input);
            cmdproc.StandardInput.WriteLine(HaLineEnd);
            this.lineEvent.Wait();
        }

        public void Dispose()
        {
            this.cmdproc?.CancelOutputRead();
            this.cmdproc?.CancelOutputRead();
            this.cmdproc?.Kill();
        }

        private void UpdateCd()
        {
            this.Run(Rem);
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

            this.cmdproc.OutputDataReceived += (sender, args) =>
            {
                var line = args.Data;
                if (line.EndsWith(this.HaStart))
                {
                    this.startEvent.Set();
                    return;
                }

                if (line.EndsWith(this.HaLineEnd))
                {
                    this.CurrentDir = line.Substring(0, line.Length-this.HaLineEnd.Length - 1);
                    this.lineEvent.Set();
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

            this.Run(this.HaStart);
            this.startEvent.Wait();
            UpdateCd();
            this.console.WriteLine("Ready.");
        }
    }
}
