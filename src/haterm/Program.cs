namespace haterm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var console = CmdTerminal.Instance;
            var shell = new CmdShell(console);
            var mc = new Haterm(console, shell);

            mc.Run();
        }
    }
}
