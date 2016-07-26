using System;

namespace haterm
{
    public class CmdTerminal : ITerminal
    {
        public static CmdTerminal Instance { get; } = new CmdTerminal();

        private CmdTerminal()
        {
            // Console.BackgroundColor = ConsoleColor.DarkGray;
            // Console.ForegroundColor = ConsoleColor.White;
            // Console.Clear();
        }

        public int Height => Console.BufferHeight;
        public int Width => Console.BufferWidth;
        public int CursorTop => Console.CursorTop;
        public int CursorLeft => Console.CursorLeft;

        public void SetCursorPosition(int cursorTop, int cursorLeft)
            => Console.SetCursorPosition(cursorLeft, cursorTop);

        public void Write(char data)
            => Console.Write(data);

        public void Write(string data)
            => Console.Write(data);

        public void Write1(params TextBlock[] blocks)
        {
            var cf = Console.ForegroundColor;
            var cb = Console.BackgroundColor;
            foreach (var textBlock in blocks)
            {
                if (textBlock.Foreground.HasValue)
                {
                    Console.ForegroundColor = textBlock.Foreground.Value;
                }

                if (textBlock.Background.HasValue)
                {
                    Console.BackgroundColor = textBlock.Background.Value;
                }

                Console.Write(textBlock.Text);
            }

            Console.ForegroundColor = cf;
            Console.BackgroundColor = cb;
        }

        public void WriteLine(string data)
            => Console.WriteLine(data);

        public void ClearLine()
        {
            Console.Write('\r');
            Console.Write(new string(' ', this.Width - 1 ));
            Console.Write('\r');
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        public void Backspace()
        {
           Console.Write("\b \b");
        }

        public ConsoleKeyInfo ReadKey()
            => Console.ReadKey(true);
    }
}
