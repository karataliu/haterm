using System;
using System.Diagnostics;

namespace haterm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConsole console = CmdConsole.Instance;
            Console.WriteLine(console.ToDebugInfo());

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            //while (!process.StandardOutput.EndOfStream)
            //{
            //    string line = process.StandardOutput.ReadLine();
            //    Console.WriteLine(line);
            //}
        }
    }
}
