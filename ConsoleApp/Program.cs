using Networks;
using System.Threading;
using Utilities;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Entrance entrance = new Entrance(
                "127.0.0.1",
                8080,
                null
            );

            Clew.Info("QAQ");

            Traveler traveler = entrance.Create(
                "127.0.0.1",
                8080
            );

            while (true)
            {
                Thread.Sleep(10);
            }
        }
    }
}
