using System;

namespace haterm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConsole console = CmdConsole.Instance;

            while (true)
            {
                Console.WriteLine(console.ToDebugInfo());
                Console.ReadLine();
            }
        }
    }
}
