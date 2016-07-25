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
            StringBuilder lb = new StringBuilder();

            while (true)
            {
                var key = console.ReadKey();
                if (key.KeyChar == 13)
                {
                    shell.Run(lb.ToString());
                    lb.Clear();
                }
                else
                {
                    lb.Append(key.KeyChar);
                    console.Write(key.KeyChar);
                }
            }

        }
    }
}
