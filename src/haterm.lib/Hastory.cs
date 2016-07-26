using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace haterm
{
    public class HistoryManager : IDisposable
    {
        private Hastory ha = new Hastory();
        private int id = -1;
        private string lastSearch = null;

        public HistoryManager()
        {
            ha.Load();
        }

        public void ClearState()
        {
            id = -1;
        }

        public void Add(string line)
        {
            this.ha.Add(line);
        }

        public string Search(string line, bool backward)
        {
            if (line != lastSearch)
            {
                this.ClearState();
                lastSearch = line;
            }

            var result = ha.Search(line, id, backward);
            if (result == SearchCombo.NotFound)
            {
                return null;
            }
            else
            {
                id = result.Id;
                return result.Line;
            }
        }

        public void Dispose()
        {
            this.ha.Save();
        }
    }


    public class SearchCombo
    {
        public static SearchCombo NotFound = new SearchCombo();

        public string Line { get; set; }
        public int Id { get; set; }
    }

    public class Hastory
    {
        private const string defaultPrefix = "ha";
        private List<string> list = new List<string>();
        private string filepath;

        public Hastory() :
            this(HatermConfig.Instance.Id, defaultPrefix)
        {
        }

        public Hastory(Guid id, string filePrefix)
        {
            var dir = HatermConfig.Instance.Dir;
            filepath = $"{dir}\\{filePrefix}-{id}";
        }

        public void Load()
        {
            try
            {
                list = File.ReadAllLines(filepath).ToList();
            }
            catch (FileNotFoundException)
            {
            }
        }

        public void Add(string line)
        {
            list.Add(line);
        }

        public void Save()
        {
            File.WriteAllLines(filepath, list);
        }

        public int Count => list.Count;

        public SearchCombo Search(string prefix, int id = -1, bool backward = true)
        {
            if (id == -1)
            {
                id = this.list.Count;
            }

            if (backward)
            {
                for (var i = id - 1; i >= 0; i--)
                {
                    if (list[i].StartsWith(prefix))
                    {
                        return new SearchCombo()
                        {
                            Line = list[i],
                            Id = i
                        };
                    }
                }
            }
            else
            {
                for (var i = id + 1; i < list.Count; i++)
                {
                    if (list[i].StartsWith(prefix))
                    {
                        return new SearchCombo()
                        {
                            Line = list[i],
                            Id = i
                        };
                    }
                }
            }

            return SearchCombo.NotFound;
        }
    }
}
