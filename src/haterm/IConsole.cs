namespace haterm
{
    public interface IConsole
    {
        int Width { get; }
        int Height { get; }
    }

    public static class ExtensionMethods
    {
        public static string ToDebugInfo(this IConsole console)
        {
            return $"Haconsole Width:{console.Width}, Height:{console.Height}";
        }
    }
}
