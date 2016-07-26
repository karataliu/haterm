using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace haterm
{
    public class Haterm
    {
        private readonly ITerminal _terminal;
        private readonly IShell shell;
        private CmdRender rd = new CmdRender();
        private StringBuilder lb = new StringBuilder();

        private Dictionary<ConsoleKey, Action> dic;
        private Dictionary<ConsoleKey, Action> ctrlDic;


        public Haterm(ITerminal _terminal, IShell shell)
        {
            this._terminal = _terminal;
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
            this._terminal.ClearScreen();
            this.WritePrompt();
        }

        private void OnEnter()
        {
            this._terminal.WriteLine("");
            this.shell.Run(lb.ToString());
            lb.Clear();
            if (!this.shell.Exited)
            {
                this.WritePrompt();
            }
        }

        private void OnBackspace()
        {
            if (lb.Length > 0) lb.Remove(lb.Length - 1, 1);
            if (this._terminal.CursorLeft > this.Prompt.Length) this._terminal.Backspace();
        }

        private void OnWhitespace()
        {
            lb.Append(' ');
            var ctx=rd.Render(lb.ToString());
            this._terminal.ClearLine();
            this.WritePrompt();
            this._terminal.Write1(ctx.ToArray());
        }

        public string Prompt
            => this.shell.CurrentDir + "=>";

        private void WritePrompt()
        {
            this._terminal.Write(this.Prompt);
            Console.Out.Flush();
        }

        public void Run()
        {
            this.WritePrompt();
            while (!this.shell.Exited)
            {
                var key = _terminal.ReadKey();
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
                else if (key.KeyChar != 0 && key.Modifiers == 0)
                {
                    lb.Append(key.KeyChar);
                    this._terminal.Write(key.KeyChar);
                }
            }
        }
    }
}
