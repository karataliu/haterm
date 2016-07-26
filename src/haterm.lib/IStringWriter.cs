namespace haterm
{
    public interface IDualOutput
    {
        void OutWriteLine(string line);
        void ErrWriteLine(string line);
    }
}
