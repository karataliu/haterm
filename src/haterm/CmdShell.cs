using System;
using System.Diagnostics;
using System.Text;

namespace haterm
{
    public class CmdShell : IShell, IDisposable
    {
        private Process cmdproc;
        private ProcessThread mainThread;
        private StringBuilder buffer = new StringBuilder();

        public CmdShell()
        {
            CmdInit();
        }

        public string Run(string input)
        {
            cmdproc.StandardInput.WriteLine(input);
            return GetCurrentOutput();
        }

        public void Dispose()
        {
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
                CreateNoWindow = true
            };

            cmdproc = Process.Start(startInfo);

            foreach (ProcessThread thread in cmdproc.Threads)
            {
                mainThread = thread;
                break;
            }

            this.cmdproc.OutputDataReceived += Cmdproc_OutputDataReceived;

            this.cmdproc.BeginOutputReadLine();
            var b = GetCurrentOutput();
      //  this.cmdproc.CancelOutputRead();
        }

        private void Cmdproc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            buffer.Append(e.Data);
        }

        private string GetCurrentOutput()
        {
            while (mainThread.ThreadState != ThreadState.Wait)
            {
                System.Threading.Thread.Sleep(1000);
            }

            var str = buffer.ToString();
            buffer.Clear();
           // this.cmdproc.CancelOutputRead();
            return str;
        }
    }
}
