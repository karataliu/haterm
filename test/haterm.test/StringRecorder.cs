using System.Collections.Generic;

namespace haterm.test
{
    internal class StringRecorder : IStringWriter
    {
        public IList<string> List => list;

        private List<string> list= new List<string>();

        public void WriteLine(string line)
        {
            list.Add(line);
        }
    }
}
