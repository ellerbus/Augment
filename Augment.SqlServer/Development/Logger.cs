using System;

namespace Augment.SqlServer.Development
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

        public static void Note(string message)
        {
            ConsoleColor c = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(message);

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
