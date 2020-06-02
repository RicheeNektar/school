using System;

namespace Test.InputAPI
{
    public class Defaults
    {
        private static ConsoleColor _fg, _bg;

        public static void GetColors()
        {
            _fg = Console.ForegroundColor;
            _bg = Console.BackgroundColor;
        }

        public static ConsoleColor Foreground => _fg;
        public static ConsoleColor Background => _bg;
    }
}