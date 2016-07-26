using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace haterm
{
    public class HintItem
    {
        public string Category { get; set; }
        public string Word { get; set; }
    }


    class HatermHint
    {
        public IEnumerable<HintItem> getHint(string path, string line)
        {
            if (line.StartsWith("git "))
            {
                return getGitHint("");
            }
            else
            {
                var pat = line.Split(' ').Last();

                return getDirHint(path, pat);
            }
        }

        private string[] gitCmds = new string[] {"branch", "checkout", "clone"};

        public IEnumerable<HintItem> getGitHint(string pattern)
        {
            foreach (var cmd in gitCmds)
            {
                if (cmd.StartsWith(pattern))
                {
                    yield return new HintItem
                    {
                        Category = "git-cmd",
                        Word = cmd,
                    };
                }
            }
        }

        public IEnumerable<HintItem> getDirHint(string path, string pattern)
        {
            var list = new List<HintItem>();
            var di = new DirectoryInfo(path);
            foreach (var item in di.EnumerateDirectories(pattern + "*"))
            {
                list.Add(new HintItem
                {
                    Category = "Directory",
                    Word = item.Name,
                });
            }

            foreach (var item in di.EnumerateFiles(pattern + "*"))
            {
                list.Add(new HintItem
                {
                    Category = "File",
                    Word = item.Name,
                });
            }

            return list;
        }
    }
}
