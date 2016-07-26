using System;

namespace haterm
{
    public interface ITerminal
    {
        int Height { get; }
        int Width { get; }
        int CursorTop { get; }
        int CursorLeft { get; }
        void SetCursorPosition(int cursorTop, int cursorLeft);
        void Write(char data);
        void Write(string data);
        void Write1(params TextBlock[] blocks);
        void WriteLine(string data);
        void ClearLine();
        void ClearScreen();
        void Backspace();
        ConsoleKeyInfo ReadKey();
    }

    public static class ExtensionMethods
    {
        public static string ToDebugInfo(this ITerminal _terminal)
        {
            return $"Haconsole Height:{_terminal.Height},Width:{_terminal.Width},CTop:{_terminal.CursorTop},CLeft:{_terminal.CursorLeft}";
        }
    }
}
