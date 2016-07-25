using System;

namespace haterm
{
    public class CmdConsole : IConsole
    {
        public static CmdConsole Instance { get; } = new CmdConsole();

        private CmdConsole() { }

        public int Height => Console.BufferHeight;
        public int Width => Console.BufferWidth;
        public int CursorTop => Console.CursorTop;
        public int CursorLeft => Console.CursorLeft;

        public void SetCursorPosition(int cursorTop, int cursorLeft)
            => Console.SetCursorPosition(cursorLeft, cursorTop);

        public void Write(string data)
            => Console.Write(data);
    }
}
