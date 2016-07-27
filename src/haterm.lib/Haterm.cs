using System;
using System.Collections.Generic;
using System.Linq;

namespace haterm
{
    public class Haterm : IDisposable
    {
        private readonly ITerminal _terminal;
        private readonly IShell shell;

        private HatermHint hh = new HatermHint();
        private HistoryManager hm = new HistoryManager();
        private AliasExpander ae = new AliasExpander();
        private CmdRender rd = new CmdRender();
        private LineBuffer lb = new LineBuffer();

        private Dictionary<ConsoleKey, Action> dic;
        private Dictionary<ConsoleKey, Action> ctrlDic;

        private int promptLen;

        public Haterm(ITerminal _terminal, IShell shell)
        {
            this._terminal = _terminal;
            this.shell = shell;
            dic = new Dictionary<ConsoleKey, Action>
            {
                {ConsoleKey.Enter       , this.OnEnter          },
                {ConsoleKey.Backspace   , this.OnBackspace      },
                {ConsoleKey.Spacebar    , this.OnWhitespace     },
                {ConsoleKey.UpArrow     , this.BackSearch       },
                {ConsoleKey.DownArrow   , this.ForwardSearch    },
                {ConsoleKey.LeftArrow   , this.MoveBack         },
                {ConsoleKey.RightArrow  , this.MoveForward      },
                {ConsoleKey.Tab         , this.Hint             },
            };

            ctrlDic = new Dictionary<ConsoleKey, Action>
            {
                {ConsoleKey.L           , this.Clear        },
                {ConsoleKey.D           , this.Exit         },
                {ConsoleKey.A           , this.Expand       },
                {ConsoleKey.E           , this.ShowDebug    },
            };
        }

        private void ClearHint()
        {
            if (this.maxhint > 0)
            {
                this._terminal.PushCursor();
                while (maxhint-- > 0)
                {
                    this._terminal.SetCursorPosition(this._terminal.CursorTop + 1, 0);
                    Console.Write(new string(' ', this._terminal.Width - 1));
                }
                this._terminal.PopCursor();
            }
        }

        private int maxhint = 0;

        private void Hint()
        {
            this.Expand();

            string lw = null;

            this.ClearHint();
            this._terminal.PushCursor();
            var he1 = hh.getHint(this.shell.CurrentDir, this.lb.Line);
            var ng = he1.Groups;
            maxhint = 0;
            this._terminal.WriteLine("");
            foreach (var item in ng)
            {
                this._terminal.WriteLine($"---{item.Key}--");
                maxhint++;
                foreach (var word in item)
                {
                    this._terminal.WriteLine($"{word.Word} ");
                    maxhint++;
                    lw = word.Word;
                }
            }

            this._terminal.PopCursor();

            if (he1.ResultCount > 0)
            {
                this.lb.ReplaceLastSegment(he1.CommonPrefix);
                // this.ClearHint();
            }

            this.RenderCurrentLine();
        }

        private void ShowDebug()
        {
            this._terminal.WriteLine("");
            this._terminal.WriteLine($"DEBUG:cd:{this.shell.CurrentDir}");
            this.RenderCurrentLine();
        }

        private void MoveBack()
        {
            if (lb.Back())
            {
                UpdateCursor();
            }
        }

        private void MoveForward()
        {
            if (lb.Forward())
            {
                UpdateCursor();
            }
        }

        private void UpdateCursor()
        {
            this._terminal.SetCursorPosition(this._terminal.CursorTop, promptLen + this.lb.CurrentIndex);
        }

        private void BackSearch()
        {
            this.search(true);
        }

        private void ForwardSearch()
        {
            this.search(false);
        }

        private void search(bool backward)
        {
            var index = lb.CurrentIndex;
            var result = this.hm.Search(lb.LineToCur, backward);
            if (result != null)
            {
                this.lb.Replace(result);
            }

            this.RenderCurrentLine();
            lb.CurrentIndex = index;
            this.UpdateCursor();
        }

        private void Expand()
        {
            this.lb.Replace(ae.Expand(lb.Line));

            if (!this.lb.Line.Contains(' '))
            {
                this.lb.Replace(ae.ExpandDir(this.shell.CurrentDir, lb.Line));
            }

            RenderCurrentLine();
        }

        private void Exit()
        {
            this.shell.Exit();
        }

        private void Clear()
        {
            this._terminal.ClearScreen();
            RenderCurrentLine();
        }

        private void RenderCurrentLine()
        {
            var line = lb.Line;
            this._terminal.ClearLine();
            this.WritePrompt();
            var ctx=rd.Render(line);
            this._terminal.Write1(ctx.ToArray());
        }

        private string lastLine = "'";

        private void OnEnter()
        {
            this.ClearHint();
            this.hm.ClearState();
            this.Expand();
            this._terminal.WriteLine("");

            var line = lb.Line;
            lb.Replace("");
            lastLine = line;
            this.hm.Add(line);
            this.shell.Run(line);

            if (!this.shell.Exited)
            {
                this.RenderCurrentLine();
            }
        }

        private void OnBackspace()
        {
            lb.Backspace();
            if (this._terminal.CursorLeft > this.Prompt.Length)
            {
                this._terminal.Backspace();

            }
        }

        private void OnWhitespace()
        {
            lb.Add(' ');
            this.RenderCurrentLine();
        }

        public string Prompt
            => this.shell.CurrentDir + "=>";

        private void WritePrompt()
        {
            this._terminal.Write(this.Prompt);
            this.promptLen = this.Prompt.Length;
            Console.Out.Flush();
        }

        private bool escaped = false;

        public void Run()
        {
            this.RenderCurrentLine();
            while (!this.shell.Exited)
            {
                var key = _terminal.ReadKey();

                if (this.escaped)
                {
                    this.escaped = false;
                    if (key.KeyChar == '.')
                    {
                        this.lb.Add(lastLine.Split(' ').Last());
                        this.RenderCurrentLine();
                    }

                    continue;
                }

                Action action;
                IDictionary<ConsoleKey, Action> lookup = dic;
                if (key.Modifiers == ConsoleModifiers.Control)
                {
                    lookup = ctrlDic;
                }

                if (lookup.TryGetValue(key.Key, out action))
                {
                    action();
                }else if (key.KeyChar == 27)
                {
                    escaped = true;
                }
                else if (key.KeyChar != 0)
                {
                    if (key.Modifiers == 0 || key.Modifiers == ConsoleModifiers.Shift)
                    {
                        lb.Add(key.KeyChar);
                        this._terminal.Write(key.KeyChar);
                    }
                }
            }
        }

        public void Dispose()
        {
            this.hm.Dispose();
        }
    }
}
