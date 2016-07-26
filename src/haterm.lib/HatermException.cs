using System;

namespace haterm
{
    public class HatermException : Exception
    {
        public HatermException(string message)
            : base(message)
        {
        }
    }
}
