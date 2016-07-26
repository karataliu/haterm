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
