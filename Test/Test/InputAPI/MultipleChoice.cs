using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;
using Test.InputAPI;

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
                    Console.ForegroundColor = Defaults.Background;
                    Console.BackgroundColor = Defaults.Foreground;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.WriteLine(strings[i]);
            }
        }

        public static int Show(string title, params string[] choices)
        {
            int selected = 0;
            ConsoleKeyInfo info;
            int cursorTop = Console.CursorTop;

            do
            {
                Console.SetCursorPosition(0, cursorTop);
                RenderList(title, choices, selected);
                Console.ResetColor();

                info = Console.ReadKey(true);

                switch (info.Key)
                {
                    case ConsoleKey.UpArrow:
                        selected--;
                        if (selected < 0) selected = choices.Length - 1;
                        break;
                    
                    case ConsoleKey.DownArrow:
                        selected++;
                        if (selected >= choices.Length) selected = 0;
                        break;
                }
                
            } while (info.Key != ConsoleKey.Enter || string.IsNullOrEmpty(choices[selected]));

            Console.ResetColor();
            return selected;
        }
    }
}
