using System.Collections.Generic;
using System.IO;

namespace haterm
{
    public class AliasExpander
    {
        private Dictionary<string,string> dic = new Dictionary<string, string>
        {
            {"G", "|findstr /I"     },
            {"gb", "git branch"     },
            {"gc", "git checkout"   },
        };

        public string Expand(string input)
        {
            var output = input;
            foreach (var item in dic)
            {
                if (output.StartsWith(item.Key))
                {
                    output = output.Replace(item.Key, item.Value);
                }
                else
                {
                    output = output.Replace($" {item.Key} ", $" {item.Value} ");
                }
            }

            return output;
        }

        public string ExpandDir(string cwd, string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var output = input;
            var path = Path.Combine(cwd, input);
            if (Directory.Exists(path))
            {
                return $"cd {input}";
            }else if (File.Exists(path))
            {
                return $"start {input}";
            }

            return output;
        }
    }
}
