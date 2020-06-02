using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.InputAPI
{
    class NameEditor
    {
        private static void RenderList(string[] names, int editing, int whiteSpace)
        {
            for(int i = 0; i<names.Length; i++)
            {
                if(i == editing)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                string name = names[i];
                int whiteSpaces = whiteSpace - name.Length;
                
                Console.Write("Player {0} : {1}", (i + 1), name);
                if(whiteSpaces > 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.Write(new string(new char[whiteSpaces]).Replace('\0', ' '));
                }

                Console.WriteLine();
            }
        }

        public static string[] RequestBatch(int batchSize)
        {
            int cursorTop = Console.CursorTop;
            ConsoleKeyInfo info;

            int cursorLeft = 0;
            int editing = 0;
            int longest = 0;

            string[] names = new string[batchSize];

            do
            {
                Console.SetCursorPosition(0, cursorTop);
                RenderList(names, editing, longest);

                foreach (string name in names)
                {
                    longest = Math.Max(longest, name.Length);
                }

                info = Console.ReadKey(true);

                switch(info.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (editing > 0) editing--;
                        break;

                    case ConsoleKey.DownArrow:
                        if (editing < batchSize) editing++;
                        break;

                    case ConsoleKey.LeftArrow:
                        break;

                    case ConsoleKey.RightArrow:
                        break;

                    case ConsoleKey.Backspace:
                        string s = names[editing];
                        if (s.Length > 0)
                        {
                            names[editing] = s.Substring(0, s.Length - 1);
                        }
                        break;

                    default:
                        if(char.IsLetterOrDigit(info.KeyChar))
                        {
                            names[editing] += info.KeyChar;
                        }
                        break;
                }

            } while (info.Key != ConsoleKey.Enter);

            Console.ResetColor();
            return names;
        }
    }
}
