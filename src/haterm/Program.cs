using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
        private Dictionary<ConsoleKey, Action> ctrlDic;


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

            ctrlDic = new Dictionary<ConsoleKey, Action>
            {
                {ConsoleKey.L           , this.Clear        },
            };
        }

        private void Clear()
        {
            this.console.ClearScreen();
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
            this.console.Backspace();
        }

        private void OnWhitespace()
        {
            lb.Append(' ');
            var ctx=rd.Render(lb.ToString());
            this.console.ClearLine();
            this.console.Write1(ctx.ToArray());
        }

        public void Run()
        {
            while (true)
            {
                var key = console.ReadKey();
                Action action;
                IDictionary<ConsoleKey, Action> lookup = dic;
                if (key.Modifiers == ConsoleModifiers.Control)
                {
                    lookup = ctrlDic;
                }

                if (lookup.TryGetValue(key.Key, out action))
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
            var console = CmdConsole.Instance;
            var shell = new CmdShell(console);
            var mc = new MyConsole(console, shell);

            mc.Run();
        }
    }
}
