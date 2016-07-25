using System;
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
        private CmdRender rd = new CmdRender();
        private StringBuilder lb = new StringBuilder();

        private Dictionary<ConsoleKey, Action> dic;

        public MyConsole(IConsole console, IShell shell)
        {
            this.console = console;
            this.shell = shell;
            dic = new Dictionary<ConsoleKey, Action>
            {
                {ConsoleKey.Enter       , this.OnEnter      },
                {ConsoleKey.Backspace   , this.OnBackspace  },
                {ConsoleKey.Spacebar    , this.OnWhitespace },
            };
        }

        private void OnEnter()
        {
            this.shell.Run(lb.ToString());
            lb.Clear();
            this.console.WriteLine(this.shell.CurrentDir);
        }

        private void OnBackspace()
        {
            lb.Remove(lb.Length - 1, 1);
            this.console.Write("\b \b");
        }

        private void OnWhitespace()
        {
            lb.Append(' ');
            var ctx=rd.Render(lb.ToString());
            this.console.Write('\r');
            this.console.Write1(ctx.ToArray());
        }

        public void Run()
        {
            while (true)
            {
                var key = console.ReadKey();
                Action action;
                if (dic.TryGetValue(key.Key, out action))
                {
                    action();
                }
                else
                {
                    lb.Append(key.KeyChar);
                    this.console.Write(key.KeyChar);
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
            
            var mc = new MyConsole(console, shell);

            mc.Run();
        }
    }
}
