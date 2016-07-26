namespace haterm
{
    public interface IShell
    {
        bool Exited { get; }
        string CurrentDir { get; }
        void Run(string input);
        void Exit();
    }
}
