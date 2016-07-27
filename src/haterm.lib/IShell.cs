using System;

namespace haterm
{
    public interface IShell : IDisposable
    {
        bool Exited { get; }
        string CurrentDir { get; }
        void Run(string input);
        void Exit();
    }
}
