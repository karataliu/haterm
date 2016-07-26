using System;
using System.IO;

namespace haterm
{
    class TextWriterWrapper : IStringWriter
    {
        private TextWriter textWriter;
        public TextWriterWrapper(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        public void WriteLine(string line)
        {
            this.textWriter.WriteLine(line);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var shell = new CmdShell(new TextWriterWrapper(Console.Out), new TextWriterWrapper(Console.Error)))
            {
                var mc =new Haterm(CmdTerminal.Instance, shell);
                mc.Run();
            }
        }
    }
}
