using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace haterm
{
    public class CmdShell : IShell, IDisposable
    {
        private const int startTimeout = 2000;
        private const int defaultTimeout = 60 * 1000;
        private const string Rem = "rem";
        private const string Rem1 = "rem1";
        private readonly string HaRunId = Guid.NewGuid().ToString();
        private readonly string HaLineEnd;
        private readonly string ErrHaLineEnd;
        private readonly string ErrHaLineEndOut;
        private readonly Process cmdproc;
        private readonly ManualResetEventSlim lineEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim errLineEvent = new ManualResetEventSlim(false);
        private readonly IStringWriter Output;
        private readonly IStringWriter Error;
        private List<string> outputBuffer = new List<string>();


        public bool Exited => this.cmdproc.HasExited;

        public string CurrentDir { get; private set; }

        public CmdShell(IStringWriter output, IStringWriter error)
        {
            this.Output = output;
            this.Error = error;
            HaLineEnd = $"{Rem} {HaRunId}{nameof(HaLineEnd)}";
            ErrHaLineEnd = $"{Rem1} {HaRunId}{nameof(HaLineEnd)}";
            ErrHaLineEndOut = $"'{Rem1}' is not recognized as an internal or external command,";

            this.cmdproc = CreateCmdProcess();
            this.UpdateCwd(startTimeout);
            this.FlushOutput();
            this.Output.WriteLine(Constants.Branding);
            this.Output.WriteLine("");
        }

        public void Run(string input)
        {
            cmdproc.StandardInput.WriteLine(input);
            cmdproc.StandardInput.WriteLine(ErrHaLineEnd);
            this.errLineEvent.Wait();
            this.errLineEvent.Reset();
            FlushOutput();
            UpdateCwd();
        }

        private void UpdateCwd(int timeOut = defaultTimeout)
        {
            cmdproc.StandardInput.WriteLine(HaLineEnd);
            if (this.lineEvent.Wait(timeOut))
            {
                this.lineEvent.Reset();
                if (Exited) return;
            }
            else
            {
                throw new HatermException("Update Cwd timeout.");
            }
        }

        public void Dispose()
        {
            if (this.cmdproc != null && !this.cmdproc.HasExited)
            {
                this.cmdproc.CancelOutputRead();
                this.cmdproc.CancelOutputRead();
                this.cmdproc.Kill();
            }
        }

        private int skipLine = 0;

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs args)
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

            
            if (line.EndsWith(this.ErrHaLineEnd))
            {
                skipLine = 1;
                return;
            }

            outputBuffer.Add(line);
        }

        private void FlushOutput()
        {
            foreach (var line in outputBuffer)
            {
                Output.WriteLine(line);
            }
            outputBuffer.Clear();
        }

        private int errSkipLine = 0;
        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            var line = args.Data;
            if (line == null)
            {
                return;
            }

            if (errSkipLine > 0)
            {
                --errSkipLine;
                if (errSkipLine == 0)
                {
                    this.errLineEvent.Set();
                }

                return;
            }

            if (line.Equals(ErrHaLineEndOut))
            {
                errSkipLine = 1;
                return;
            }

            Error.WriteLine("E:" + line);
        }

        private Process CreateCmdProcess()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true,
            };

            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Exited += (sender, args) =>
            {
                this.Output.WriteLine("Shell exited.");
                this.errLineEvent.Set();
                this.lineEvent.Set();
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }

    }
}
