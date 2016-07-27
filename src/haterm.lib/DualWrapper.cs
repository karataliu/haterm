using System;

namespace haterm
{
    public class DualWrapper : IDualOutput
    {
        public DualWrapper()
        {
        }

        public void OutWriteLine(string line)
        {
            if (line == "\f")
            {
                Console.Clear();
                return;
            }
            Console.Out.WriteLine(line);
        }

        public void ErrWriteLine(string line)
        {
            Console.Error.WriteLine(line);
        }
    }
}
