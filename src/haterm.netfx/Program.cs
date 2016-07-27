namespace haterm.netfx
{
    class Program
    {
        public static void Main(string[] args)
        {
            IShell shell = null;
            try
            {
                shell = new CmdShell(new DualWrapper());
                using (var  haterm = new Haterm(CmdTerminal.Instance, shell))
                {
                    shell = null;
                    haterm.Run();
                }
            }
            finally
            {
                if (shell != null)
                    shell.Dispose();
            }
        }
    }
}
