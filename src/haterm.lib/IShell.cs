namespace haterm
{
    public interface IShell
    {
        string CurrentDir { get; }
        void Run(string input);
    }
}
