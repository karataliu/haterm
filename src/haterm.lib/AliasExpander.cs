using System.Collections.Generic;

namespace haterm
{
    public class AliasExpander
    {
        private Dictionary<string,string> dic = new Dictionary<string, string>
        {
            {"G", "|findstr /I" }
        };

        public string Expand(string input)
        {
            var output = input;
            foreach (var item in dic)
            {
                output = output.Replace($" {item.Key} ", $" {item.Value} ");
            }

            return output;
        }
    }
}
