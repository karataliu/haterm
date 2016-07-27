using System;
using System.Collections.Generic;
using System.IO;

namespace haterm
{
    public class HatermConfig
    {
        public static HatermConfig Instance = new HatermConfig();

        private string cfg;

        private HatermConfig()
        {
            var home = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            Dir = $"{home}\\.haterm";
            Directory.CreateDirectory(Dir);
            cfg = $"{Dir}\\config";
            bool idSet = false;


            try
            {
                var list = File.ReadAllLines(cfg);
                foreach (var item in list)
                {
                    var index = item.IndexOf('=');
                    if (index < 0) continue;

                    var key = item.Substring(0, index);
                    var val = item.Substring(index + 1);

                    if (string.Equals(key, nameof(Id), StringComparison.OrdinalIgnoreCase))
                    {
                        Id = Guid.Parse(val);
                        idSet = true;
                    }
                    else if (string.Equals(key, nameof(GitPath), StringComparison.OrdinalIgnoreCase))
                    {
                        GitPath = val;
                    }
                    else if (string.Equals(key, nameof(DbString), StringComparison.OrdinalIgnoreCase))
                    {
                        DbString = val;
                    }
                }
            }
            catch (Exception)
            {
                this.GenNewFile();
            }

            if (!idSet)
            {
                this.GenNewFile();
            }
        }

        private void GenNewFile()
        {
            Id = Guid.NewGuid();
            GitPath = @"C:\Program Files\Git\bin\git.exe";
            var list = new List<string>
                {
                    $"Id={Id}",
                    $"GitPath={GitPath}",
                    $"DbString={DbString}",
                };

            File.WriteAllLines(cfg, list);
        }

        public string Dir { get; private set; }

        public Guid Id { get; private set; }
        public string GitPath { get; private set; }

        public string DbString { get; private set; }
    }
}
