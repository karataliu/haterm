using System;
using System.Collections.Generic;
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
        private string[] supportedCmd = new[]
        {
            "dir",
            "git",
        };

        public CmdRender()
        {
        }

        public IEnumerable<TextBlock> Render(string line)
        {
            List<TextBlock> list = new List<TextBlock>();
            var items = line.Split('|');
            bool first = true;
            foreach (var item in items)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    list.Add(new TextBlock
                    {
                        Text = "|",
                        Foreground = ConsoleColor.DarkMagenta
                    });
                }

                list.AddRange(renderSingle(item));

            }

            return list;
        }

        public IEnumerable<TextBlock> renderSingle(string line)
        {
            var pattern = new Regex("^(.*?) (.*)$");
            var match = pattern.Match(line);
            if (match.Success)
            {
                var cmd = match.Groups[1];
                var parameter = match.Groups[2];

                return new[]
                {
                    new TextBlock
                    {
                        Text = cmd.Value,
                        Foreground = ConsoleColor.Green,
                    },
                    new TextBlock
                    {
                        Text = " "+ parameter.Value,
                        Foreground = ConsoleColor.Yellow,
                    }
                };
            }

            return new[] { new TextBlock
            {
                 Text = line
            }};
        }
    }
}
