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
        public delegate void ConsoleHandler(MyContext console);

        private readonly IConsole console;
        private StringBuilder lb = new StringBuilder();

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
                var ch = key.KeyChar;
                if (ch == 13)
                {
                    this.OnEnter?.Invoke(this.GetContext());
                    lb.Clear();
                }else if (ch == '\b')
                {
                    lb.Remove(lb.Length - 1, 1);
                    this.console.Write("\b \b");
                }else if (ch == '\t')
                {
                    this.OnTab?.Invoke(this.GetContext());
                }
                else if (ch == 32)
                {
                    lb.Append(ch);
                    var ctx = this.GetContext();
                    this.OnWhitespace?.Invoke(ctx);
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

            StringBuilder lb = new StringBuilder();
            var rd = new CmdRender();

            var mc = new MyConsole(console);
            mc.OnEnter += (context) =>
            {
                shell.Run(context.Line);
            };

            mc.OnWhitespace += (context) =>
            {
                context.Current = rd.Render(context.Line);
            };

            mc.Run();
        }
    }
}
