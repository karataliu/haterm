using System;
using System.Diagnostics;
using System.Text;

namespace haterm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //IConsole console = CmdConsole.Instance;
            //Console.WriteLine(console.ToDebugInfo());

         
            var shell = new CmdShell();
            var out1 = shell.Run("dir");
            Console.WriteLine(out1);

        }
    }
}
