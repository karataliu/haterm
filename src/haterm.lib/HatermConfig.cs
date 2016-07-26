using System;
using System.IO;

namespace haterm
{
    public class HatermConfig
    {
        public static HatermConfig Instance = new HatermConfig();

        private HatermConfig()
        {
            var home = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            Dir = $"{home}\\.haterm";
            Directory.CreateDirectory(Dir);
            var cfg = $"{Dir}\\config";

            try
            {
                var data = File.ReadAllText(cfg);
                Id = Guid.Parse(data);
            }
            catch (Exception)
            {
                Id = Guid.NewGuid();
                File.WriteAllText(cfg, Id.ToString());
            }
        }

        public string Dir { get; private set; }

        public Guid Id { get; private set; }

    }
}
