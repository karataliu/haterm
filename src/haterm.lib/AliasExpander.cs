using System;
using System.Collections.Generic;
using System.IO;

namespace haterm
{
    public class AliasExpander
    {
        public AliasExpander()
        {
            var file = Path.Combine(HatermConfig.Instance.Dir, "alias.txt");
            try
            {
                var list = File.ReadAllLines(file);

                foreach (var item in list)
                {
                    var index = item.IndexOf('=');
                    if (index < 0) continue;

                    var key = item.Substring(0, index);
                    var val = item.Substring(index + 1);

                    dic.Add(key, val);
                }
            }
            catch (Exception)
            {
                
            }
        }

        private Dictionary<string,string> dic = new Dictionary<string, string>
        {
            //{"G", "|findstr /I"     },
            //{"gb", "git branch"     },
            //{"gc", "git checkout"   },
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
                if (input.Contains(":"))
                    return $"cd /d {input}";

                return $"cd {input}";
            }else if (File.Exists(path))
            {
                return $"start {input}";
            }

            return output;
        }
    }
}
