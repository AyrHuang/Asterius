using Asterius;
using Asterius.Base;
using System;
using System.Threading;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Entrance a = new Entrance(
                "127.0.0.1",
                8080,
                null
            );

            Clew.Info("QAQ");

            Traveler b = new Traveler(
                "127.0.0.1",
                8080
            );

            while(true)
            {
                Thread.Sleep(10);
            }
        }
    }
}
