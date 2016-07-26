using System;
using System.Diagnostics;
using System.Threading;

namespace haterm
{
    public interface IStringWriter
    {
        void WriteLine(string line);
    }

    public class CmdShell : IShell, IDisposable
    {
        private const int startTimeout = 2000;
        private const int defaultTimeout = 60 * 1000;
        private const string Rem = "rem";
        private readonly string HaRunId = Guid.NewGuid().ToString();
        private readonly string HaLineEnd;
        private Process cmdproc;
        private ManualResetEventSlim lineEvent = new ManualResetEventSlim(false);

        private readonly IStringWriter Output;
        private readonly IStringWriter Error;

        public string CurrentDir { get; set; }

        public CmdShell(IStringWriter output, IStringWriter error)
        {
            this.Output = output;
            this.Error = error;
            HaLineEnd = $"{Rem} {HaRunId}{nameof(HaLineEnd)}";

            CmdInit();
        }

        public void Run(string input)
        {
            cmdproc.StandardInput.WriteLine(input);
            UpdateCwd();
        }

        private void UpdateCwd(int timeOut = defaultTimeout)
        {
            cmdproc.StandardInput.WriteLine(HaLineEnd);
            if (this.lineEvent.Wait(timeOut))
            {
                this.lineEvent.Reset();
            }
            else
            {
                throw new HatermException("Start timeout.");
            }
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

                Output.WriteLine(args.Data);
            };

            this.cmdproc.BeginOutputReadLine();

            this.cmdproc.ErrorDataReceived += (sender, args) =>
            {
                //this.terminal.Write1(new TextBlock
                //{
                //    Text = args.Data,
                //    Foreground = ConsoleColor.Red
                //});
                //this.terminal.WriteLine("");
                Error.WriteLine(args.Data);
            };
            this.cmdproc.BeginErrorReadLine();

            this.UpdateCwd(startTimeout);
            this.Output.WriteLine(Constants.Branding);
            this.Output.WriteLine("");
        }
    }
}
