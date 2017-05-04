using System;
using Augment.SqlServer.Models;

namespace Augment.SqlServer
{
    static class Logger
    {
        public static void Info(string message)
        {
            ConsoleColor c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine(message);

            Console.ForegroundColor = c;
        }

        public static void Dropping(SqlObject sqlObj)
        {
            ConsoleColor c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkYellow;

            Console.WriteLine($"Dropping {sqlObj.ToString()}");

            Console.ForegroundColor = c;
        }

        public static void Adding(SqlObject sqlObj)
        {
            ConsoleColor c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"Creating {sqlObj.ToString()}");

            Console.ForegroundColor = c;
        }

        public static void Registering(RegistryObject regObj)
        {
            ConsoleColor c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine($"Registering {regObj.ToString()}");

            Console.ForegroundColor = c;
        }

        public static void Error(string message)
        {
            ConsoleColor c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(message);

            Console.ForegroundColor = c;
        }
    }
}
