using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace haterm
{
    public class HintItem
    {
        public string Category { get; set; }
        public string Word { get; set; }
        public string Description { get; set; }
    }

    public class HintContext
    {
        public string CurrentDir { get; set; }
        public string Line { get; set; }
        public string LastSegment { get; set; }
    }

    public class HintResult
    {
        public IEnumerable<IGrouping<string, HintItem>> Groups { get; set; }
        public int ResultCount { get; set; }
        public string CommonPrefix { get; set; }
    }

    public class HatermHint
    {
        public static string getCommonPrefix(IList<string> list)
        {
            var len = list.Count();
            if (len == 0)
            {
                return "";
            }
            else if (len == 1)
            {
                return list[0];
            }
            else
            {
                var bas = TwoStringCommonPrefix(list[0], list[1]);
                foreach (var item in list.Skip(2))
                {
                    bas = TwoStringCommonPrefix(bas, item);
                }

                return bas;
            }
        }

        private static string TwoStringCommonPrefix(string a, string b)
        {
            int i = 0;
            while (i < a.Length && i < b.Length && a[i] == b[i]) ++i;
            return a.Substring(0, i);
        }

        private Dictionary<string, Func<HintContext, IEnumerable<HintItem>>> dic = new Dictionary<string, Func<HintContext, IEnumerable<HintItem>>>
        {
            { "git checkout ", getGitCheckoutHint},
            { "git ", getGitHint},
            { "dir ", getDirCmdHint},
            { "",   getDirHint},
        };

        public HintResult getHint(string path, string line)
        {
            IList<HintItem> items = new List<HintItem>();

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
                    items = item.Value.Invoke(ctx).ToList();
                    break;
                }
            }

            var words = items.Select(i => i.Word.ToLowerInvariant()).ToList();

            return new HintResult
            {
                Groups = items.GroupBy(item => item.Category),
                ResultCount = words.Count,
                CommonPrefix = getCommonPrefix(words)
            };
        }

        private static IEnumerable<HintItem> getGitCheckoutHint(HintContext context)
        {
            var branches = new List<string>();

            branches.AddRange(GetGitBranches(context.CurrentDir));

            return branches.Where(name => name.StartsWith(context.LastSegment)).Select(name => new HintItem
            {
                Category = "Git " + (name.Contains('/') ? "Remote" : "Local") + " Branch",
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


        private static Dictionary<string,string> gitCmds = new Dictionary<string, string>
        {
            { "add",        "Add file to index."        },
            { "branch",     "List branches."            },
            { "checkout",   "Switch between branches."  },
            { "clean",      "Remove untracked items."   },
            { "fetch",      "Download remote changes."  },
            { "init",       "Initialize a git repo."    },
        };

        private static IEnumerable<HintItem> getGitHint(HintContext context)
        {
            var pattern = context.LastSegment;
            foreach (var cmd in gitCmds)
            {
                if (cmd.Key.StartsWith(pattern))
                {
                    yield return new HintItem
                    {
                        Category = "git commandd",
                        Word = cmd.Key,
                        Description = cmd.Value,
                    };
                }
            }
        }

        private static Dictionary<string,string> dirParameters = new Dictionary<string, string>
        {
            { "/w",        "Use wide list format."      },
            { "/q",        "Display owner."             },
            { "/ad",       "List all directories."      },
            { "/ah",       "List all hidden files."     },
        };

        private static IEnumerable<HintItem> getDirCmdHint(HintContext context)
        {
            var pattern = context.LastSegment;
            return dirParameters
                .Where(item => item.Key.StartsWith(context.LastSegment))
                .Select(item => new HintItem
            {
                Category = "dir parameters",
                Word = item.Key,
                Description = item.Value,
            }).Concat(getDirHint(context));
        }

        private static IEnumerable<HintItem> getDirHint(HintContext context)
        {
            var list = new List<HintItem>();
            var pattern = context.LastSegment;

            string dirPath = context.CurrentDir;
            string pathInPattern = "";
            if (pattern.Contains("/")) return list;

            if (pattern.Contains("\\"))
            {
                var index = pattern.LastIndexOf('\\');
                pathInPattern = pattern.Substring(0, index + 1);
                pattern = pattern.Substring(index + 1);

                dirPath = Path.Combine(context.CurrentDir, pathInPattern);
                if (!Directory.Exists(dirPath))
                {
                    return Enumerable.Empty<HintItem>();
                }
            }

            var di = new DirectoryInfo(dirPath);

            foreach (var item in di.EnumerateDirectories(pattern + "*"))
            {
                list.Add(new HintItem
                {
                    Category = "Directory",
                    Word = pathInPattern + item.Name,
                });
            }

            foreach (var item in di.EnumerateFiles(pattern + "*"))
            {
                list.Add(new HintItem
                {
                    Category = "File",
                    Word = pathInPattern + item.Name,
                    Description = FileTypeHelper.FileTypeInfo(item.FullName)
                });
            }

            return list;
        }
    }
}
