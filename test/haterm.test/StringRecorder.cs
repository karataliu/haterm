using System.Collections.Generic;

namespace haterm.test
{
    internal class OutRecorder : IDualOutput
    {
        public IList<string> Out { get; } = new List<string>();
        public IList<string> Err { get; } = new List<string>();

        public void OutWriteLine(string line)
        {
            Out.Add(line);
        }

        public void ErrWriteLine(string line)
        {
            Err.Add(line);
        }
    }
}
