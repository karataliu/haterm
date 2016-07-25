using System;

namespace haterm
{
    public interface IConsole
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
        public static string ToDebugInfo(this IConsole console)
        {
            return $"Haconsole Height:{console.Height},Width:{console.Width},CTop:{console.CursorTop},CLeft:{console.CursorLeft}";
        }
    }
}
