using System.Collections.Generic;

namespace haterm
{
    public class AliasExpander
    {
        private Dictionary<string,string> dic = new Dictionary<string, string>
        {
            {"G", "|findstr /I" },
            {"gb", "git branch" },
            {"gc", "git clone" },
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
    }
}
