using Asterius;
using Asterius.Base;
using System;

namespace ConsoleApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Service a = new Service(
                "127.0.0.1",
                8080
            );

            Clew.Info("QAQ");
        }
    }
}
