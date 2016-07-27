namespace haterm.netfx
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (var shell = new CmdShell(new DualWrapper()))
            {
                var mc =new Haterm(CmdTerminal.Instance, shell);
                mc.Run();
            }
        }
    }
}
