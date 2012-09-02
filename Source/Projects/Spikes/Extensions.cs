using System;

namespace Spikes
{
    public static class Extensions
    {
        [Flags]
        public enum NewLineLocation
        {
            None = 0,
            Before = 1,
            After = 2,
            Both = Before | After
        }

        private static readonly object LockObj = new object();

        public static bool IsFlagNotSet<T>(this T value, T flag) where T : struct
        {
            var testValueNumber = Convert.ToInt64(value);
            var flagNumberValue = Convert.ToInt64(flag);

            return (testValueNumber & flagNumberValue) == 0;
        }

        public static bool IsFlagSet<T>(this T value, T flag) where T : struct
        {
            var testValueNumber = Convert.ToInt64(value);
            var flagNumberValue = Convert.ToInt64(flag);

            return (testValueNumber & flagNumberValue) != 0;
        }

        public static void PrintToConsole(this Exception ex)
        {
            var level = 0;

            var currentExpection = ex;

            WriteEquals(ConsoleColor.Red, NewLineLocation.None);

            while (currentExpection != null && level < 5)
            {
                level++;

                WriteExceptionToConsole(currentExpection, level);

                currentExpection = currentExpection.InnerException;
            }

            WriteEquals(ConsoleColor.Red, NewLineLocation.Before);
        }

        public static void WriteLineWithColor(ConsoleColor color, string message, params object[] args)
        {
            lock (LockObj)
            {
                var oldColor = Console.ForegroundColor;

                Console.ForegroundColor = color;

                Console.WriteLine(message, args);

                Console.ForegroundColor = oldColor;
            }
        }

        public static void WriteWithColor(ConsoleColor color, string message, params object[] args)
        {
            lock (LockObj)
            {
                var oldColor = Console.ForegroundColor;

                Console.ForegroundColor = color;

                Console.Write(message, args);

                Console.ForegroundColor = oldColor;
            }
        }

        private static string GetPrintableStackTrace(string spaces, Exception currentExpection)
        {
            return string.IsNullOrEmpty(currentExpection.StackTrace)
                       ? null
                       : currentExpection.StackTrace.Replace(Environment.NewLine, string.Format("{1}{0}                ", Environment.NewLine, spaces));
        }

        private static void WriteEquals(ConsoleColor color, NewLineLocation lineLocation = NewLineLocation.None)
        {
            if (lineLocation.IsFlagSet(NewLineLocation.Before)) Console.WriteLine(Environment.NewLine);

            WriteLineWithColor(color, new string('=', 125));

            if (lineLocation.IsFlagSet(NewLineLocation.After)) Console.WriteLine(Environment.NewLine);
        }

        private static void WriteExceptionToConsole(Exception currentExpection, int level)
        {
            var spaces = new string(' ', level * 4);

            Console.WriteLine(Environment.NewLine);
            WriteLineWithColor(ConsoleColor.Red, "{0}Error     : {1}", spaces, currentExpection.Message);
            WriteLineWithColor(ConsoleColor.Red, "{0}StackTrace: {1}", spaces, GetPrintableStackTrace(spaces, currentExpection));
        }
    }
}