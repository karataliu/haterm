using System;
using System.IO;

namespace haterm
{
    class DualWrapper : IDualOutput
    {
        public DualWrapper()
        {
        }

        public void OutWriteLine(string line)
        {
            if (line == "\f")
            {
                Console.Clear();
                return;
            }
            Console.Out.WriteLine(line);
        }

        public void ErrWriteLine(string line)
        {
            Console.Error.WriteLine(line);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var shell = new CmdShell(new DualWrapper()))
            {
                var mc =new Haterm(CmdTerminal.Instance, shell);
                mc.Run();
            }
        }
    }
}
