using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;

namespace Test
{
    class MultipleChoice
    {
        private static void RenderList(string title, string[] args, int highlighted)
        {
            Console.ResetColor();

            Console.WriteLine("{0} :", title);
            RenderList(args, highlighted);
        }

        private static void RenderList(string[] strings, int selected)
        {
            for(int i = 0; i<strings.Length; i++)
            {
                if (i == selected)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(strings[i]);
            }
        }

        public static int Show(string title, params string[] choices)
        {
            int selected = 0;
            ConsoleKeyInfo info;
            int line = Console.CursorTop;

            do
            {
                Console.SetCursorPosition(0, line);
                RenderList(title, choices, selected);
                Console.ResetColor();

                info = Console.ReadKey(true);

                switch (info.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (selected > 0) selected--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (selected < choices.Length-1) selected++;
                        break;
                }
            } while (info.Key != ConsoleKey.Enter);

            Console.ResetColor();
            return selected;
        }
    }
}
