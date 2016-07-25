using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace haterm
{
    public class MyContext
    {
        public string Line { get; set; }
        public IEnumerable<TextBlock> Current { get; set; }
    }

    public class MyConsole
    {
        private readonly IConsole console;
        private readonly IShell shell;
        private StringBuilder lb = new StringBuilder();

        public MyConsole(IConsole console, IShell shell)
        {
            this.console = console;
            this.shell = shell;
        }

        private void OnTab()
        {
        }

        private void OnEnter()
        {
            this.shell.Run(lb.ToString());
            lb.Clear();
            this.console.WriteLine(this.shell.CurrentDir);
        }

        private void OnWHitespace()
        {
        }

        public void Run()
        {
            while (true)
            {
                var key = console.ReadKey();
                var ch = key.KeyChar;
                if (ch == 13)
                {
                    this.OnEnter();
                }else if (ch == '\b')
                {
                    lb.Remove(lb.Length - 1, 1);
                    this.console.Write("\b \b");
                }else if (ch == '\t')
                {
                    this.OnTab();
                }
                else if (ch == 32)
                {
                    lb.Append(ch);
                    var ctx = this.GetContext();
                    this.OnWHitespace();
                    if (ctx.Current != null)
                    {
                        this.console.Write('\r');
                        this.console.Write1(ctx.Current.ToArray());
                    }
                    else
                    {
                        this.console.Write(ch);
                    }
                }
                else
                {
                    lb.Append(ch);
                    this.console.Write(ch);
                }
            }
        }

        private MyContext GetContext() => new MyContext
        {
            Line = lb.ToString()
        };
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            IConsole console = CmdConsole.Instance;
            var shell = new CmdShell(console);

            var rd = new CmdRender();

            var mc = new MyConsole(console, shell);

            mc.Run();
        }
    }
}
