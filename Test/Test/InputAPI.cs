using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;

namespace Test
{
    class InputAPI
    {
        public static string[] GetPlayerData()
        {
            return null;
        }

        public static GameType GetGameType()
        {
            int selected = ShowMultipleChoice("Choose GameType :", Enum.GetNames(typeof(GameType)));
            return (GameType)
                Enum.GetValues(typeof(GameType)).GetValue(selected);
        }

        private static void RenderList(string title, string[] args, int highlighted)
        {
            Console.WriteLine("{0} :", title);
            RenderList(args, highlighted);
        }

        private static void RenderList(string[] strings, int highlighted)
        {
            for(int i = 0; i<strings.Length; i++)
            {
                Console.WriteLine("[{0}] {1}", highlighted == i ? 'X' : ' ', strings[i]);
            }
        }

        public static int ShowMultipleChoice(string title, params string[] choices)
        {
            int selected = 0;
            ConsoleKeyInfo info;

            do
            {
                RenderList(title, choices, selected);

                info = Console.ReadKey();

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

            return selected;
        }
    }
}
