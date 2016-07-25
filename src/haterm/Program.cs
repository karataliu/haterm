using System;
using System.Diagnostics;
using System.Text;

namespace haterm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConsole console = CmdConsole.Instance;
            console.WriteLine(console.ToDebugInfo());

            var shell = new CmdShell(console);
            shell.Run("dir");

            while (true)
            {
                var key = console.ReadKey();
                console.Write("P" + key.KeyChar);
            }

        }
    }
}
