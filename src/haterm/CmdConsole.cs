using System;

namespace haterm
{
    public class CmdConsole : IConsole
    {
        public static CmdConsole Instance { get; } = new CmdConsole();

        private CmdConsole()
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
        }

        public int Height => Console.BufferHeight;
        public int Width => Console.BufferWidth;
        public int CursorTop => Console.CursorTop;
        public int CursorLeft => Console.CursorLeft;

        public void SetCursorPosition(int cursorTop, int cursorLeft)
            => Console.SetCursorPosition(cursorLeft, cursorTop);

        public void Write(string data)
            => Console.Write(data);

        public void WriteLine(string data)
            => Console.WriteLine(data);

        public ConsoleKeyInfo ReadKey()
            => Console.ReadKey(true);
    }
}
