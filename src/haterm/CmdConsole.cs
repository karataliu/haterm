using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace haterm
{
    public class CmdConsole : IConsole
    {
        public static CmdConsole Instance { get; } = new CmdConsole();

        private CmdConsole()
        {
        }

        public int Width => Console.BufferWidth;
        public int Height => Console.BufferHeight;
    }
}
