using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    public class HintContext
    {
        public string CurrentDir { get; set; }
        public string Line { get; set; }
        public string LastSegment { get; set; }
    }

    internal class HatermHint
    {
        private Dictionary<string, Func<HintContext, IEnumerable<HintItem>>> dic = new Dictionary<string, Func<HintContext, IEnumerable<HintItem>>>
        {
            { "git checkout ", getGitCheckoutHint},
            { "git ", getGitHint},
            { "",   getDirHint},
        };

        public IEnumerable<HintItem> getHint(string path, string line)
        {
            var ctx = new HintContext
            {
                CurrentDir = path,
                Line = line,
                LastSegment =  line.Split(' ').Last(),
            };

            foreach (var item in dic)
            {
                if (line.StartsWith(item.Key))
                {
                    return item.Value.Invoke(ctx);
                }
            }

            return Enumerable.Empty<HintItem>();
        }

        private static IEnumerable<HintItem> getGitCheckoutHint(HintContext context)
        {
            var branches = new List<string>();

            branches.AddRange(GetGitBranches(context.CurrentDir));

            return branches.Where(name=>name.StartsWith(context.LastSegment)).Select(name => new HintItem
            {
                Category = "Git " + (name.Contains('/') ? "Remote" : "Local") + "Branch" ,
                Word = name,
            });
        }

        private static IEnumerable<string> GetGitBranches(string path)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = HatermConfig.Instance.GitPath,
                    Arguments = "branch -a",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = path
                }
            };

            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();

                if (line.Contains("HEAD")) continue;

                yield return line.Trim('*', ' ');
            }

            proc.Dispose();
        }


        private static string[] gitCmds = new string[] {"branch", "checkout", "clone"};

        private static IEnumerable<HintItem> getGitHint(HintContext context)
        {
            var pattern = context.LastSegment;
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

        private static IEnumerable<HintItem> getDirHint(HintContext context)
        {
            var list = new List<HintItem>();
            var di = new DirectoryInfo(context.CurrentDir);
            var pattern = context.LastSegment;
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
