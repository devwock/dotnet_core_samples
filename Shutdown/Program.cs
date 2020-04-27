using System;

namespace Shutdown
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Shutdown.Run(Shutdown.Mode.ForcedShutdown);
        }
    }
}