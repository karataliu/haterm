using System;
using System.Text;

namespace haterm
{
    public class MyConsole
    {
        public delegate int ConsoleHandler(IConsole console);

        private readonly IConsole console;

        public MyConsole(IConsole console)
        {
            this.console = console;
        }

        public event ConsoleHandler OnTab;
        public event ConsoleHandler OnEnter;
        public event ConsoleHandler OnWhitespace;

        public void Run()
        {
            while (true)
            {
                var key = console.ReadKey();
                if (key.KeyChar == 13)
                {
                    var x= this.OnEnter.Invoke(this.console);
                    Console.WriteLine(x);
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            IConsole console = CmdConsole.Instance;
            var shell = new CmdShell(console);

            StringBuilder lb = new StringBuilder();
            var rd = new CmdRender(console);

            var mc = new MyConsole(console);
            mc.OnEnter += (con) =>
            {
                shell.Run("echo 1");
                return 2;
            };

            mc.Run();


            //while (true)
            //{
            //    var key = console.ReadKey();
            //    if (key.KeyChar == 13)
            //    {
            //        shell.Run(lb.ToString());
            //        lb.Clear();
            //    }
            //    else
            //    {
            //        lb.Append(key.KeyChar);
            //        console.Write(key.KeyChar);

            //        rd.Render(lb.ToString());
            //        console.Write1(new TextBlock
            //        {
            //            Text = "s3",
            //            Foreground = ConsoleColor.Yellow,
            //        });
            //    }
            //}
        }
    }
}
