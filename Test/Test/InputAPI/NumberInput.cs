﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.InputAPI
{
    class NumberInput
    {
        private const char FILLER = '-';
        private const char BAR = '|';

        private static void RenderSlider(string title, int current, int min, int max, int length)
        {
            float barPerc = (float) (current - min) / (max - min);
            int barPos = (int)(barPerc * length);

            string preChars = new string(Enumerable.Repeat(FILLER, barPos).ToArray());
            string subChars = new string(Enumerable.Repeat(FILLER, length - barPos).ToArray());
            
            Console.WriteLine("{0} :\n[{1}{2}{3}] : {4}", title, preChars, BAR, subChars, current);
        }

        public static int Show(string title, int min, int max, int start = -1)
        {
            int cursorTop = Console.CursorTop;
            int current = start > -1 ? start : min;
            ConsoleKeyInfo info;

            do
            {
                Console.SetCursorPosition(0, cursorTop);

                RenderSlider(title, current, min, max, 12);
                info = Console.ReadKey(true);

                switch (info.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (current > min) current--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (current < max) current++;
                        break;
                }
            } while (info.Key != ConsoleKey.Enter);

            Console.ResetColor();
            return current;
        }
    }
}
