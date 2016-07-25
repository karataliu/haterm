using System;
using System.Text.RegularExpressions;

namespace haterm
{
    public class TextBlock
    {
        public ConsoleColor? Background { get; set; }
        public ConsoleColor? Foreground { get; set; }
        public string Text { get; set; }
    }

    public class CmdRender
    {
        private readonly IConsole console;

        private string[] supportedCmd = new[]
        {
            "dir",
            "git",
        };

        public CmdRender(IConsole console)
        {
            this.console = console;
        }

        public void Render(string line)
        {
            var pattern = new Regex("^(.*?) (.*)$");
            var match = pattern.Match(line);
            if (match.Success)
            {
                var cmd = match.Groups[1];
                var parameter = match.Groups[2];

                //this.console.Write1($"\r5{cmd} 6{parameter}");
            }
        }
    }
}
