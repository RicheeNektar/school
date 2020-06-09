using System;
using System.Linq;

namespace Test.InputAPI
{
    class LineEditor
    {
        private static void RenderList(string title, string[] values, int editing, int whiteSpace, string[] prefixes)
        {
            Console.ResetColor();
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine(title);
            }
            
            for (int i = 0; i < values.Length; i++)
            {
                if (i == editing)
                {
                    Console.ForegroundColor = Defaults.Background;
                    Console.BackgroundColor = Defaults.Foreground;
                }
                else
                {
                    Console.ResetColor();
                }

                string value = values[i];

                if (prefixes == null)
                {
                    Console.Write("Player {0} : {1}", i + 1, value);
                }
                else
                {
                    Console.Write("{0} : {1}", prefixes[i], value);
                }

                int whiteSpaces = whiteSpace - value.Length;
                if (whiteSpaces > 0)
                {
                    Console.ResetColor();
                    Console.Write(new string(new char[whiteSpaces]).Replace('\0', ' '));
                }

                Console.WriteLine();
            }
        }

        private static void SetCursorPos(int cursorLeft, int cursorTop, int editing, bool hasTitle, string[] prefixes = null)
        {
            int left = cursorLeft;
            int top = cursorTop + editing;

            if (prefixes == null)
            {
                left += $"Player {editing + 1} : ".Length;
            }
            else
            {
                left += $"{prefixes[editing]} : ".Length;
            }

            if (hasTitle) top++;
            
            Console.SetCursorPosition(left, top);
        }

        public static string[] RequestStringBatch(string title, int batchSize, string[] prefixes = null,
            string[] startInput = null, bool allowSymbols = false)
        {
            Console.CursorVisible = true;

            bool hasTitle = !string.IsNullOrEmpty(title);

            int cursorTop = Console.CursorTop;
            ConsoleKeyInfo info;

            int cursorLeft = 0;
            int editing = 0;
            int longest = 0;

            string[] values = startInput ?? Enumerable.Repeat("", batchSize).ToArray();

            do
            {
                string value = values[editing];
                foreach (string s in values)
                {
                    longest = Math.Max(longest, s.Length);
                }

                if (cursorLeft > value.Length)
                {
                    cursorLeft = value.Length;
                }

                Console.SetCursorPosition(0, cursorTop);
                RenderList(title, values, editing, longest, prefixes);

                SetCursorPos(cursorLeft, cursorTop, editing, hasTitle, prefixes);
                info = Console.ReadKey(true);

                switch (info.Key)
                {
                    case ConsoleKey.Escape:
                        Console.CursorVisible = false;
                        return null;

                    #region Movement

                    case ConsoleKey.UpArrow:
                        if (editing > 0) editing--;
                        break;

                    case ConsoleKey.DownArrow:
                        if (editing < batchSize - 1) editing++;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (cursorLeft > 0) cursorLeft--;
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorLeft < value.Length) cursorLeft++;
                        break;

                    #endregion

                    #region Backspace

                    case ConsoleKey.Backspace:
                        if (cursorLeft > 0)
                        {
                            if (cursorLeft == value.Length)
                            {
                                values[editing] = value.Substring(0, value.Length - 1);
                            }
                            else
                            {
                                string start = value.Substring(0, cursorLeft - 1);
                                string end = value.Substring(cursorLeft, value.Length - cursorLeft);

                                values[editing] = start + end;
                            }

                            cursorLeft--;
                        }

                        break;

                    #endregion

                    #region Entf

                    case ConsoleKey.Delete:
                        if (cursorLeft < value.Length)
                        {
                            string start = value.Substring(0, cursorLeft);
                            string end = value.Substring(cursorLeft + 1, value.Length - cursorLeft - 1);

                            values[editing] = start + end;
                        }

                        break;

                    #endregion

                    #region writing

                    default:
                        bool IsAllowedChar = !char.IsControl(info.KeyChar) && allowSymbols;
                        
                        if (char.IsLetterOrDigit(info.KeyChar)
                            || info.KeyChar == ' '
                            || IsAllowedChar
                        ) {
                            if (cursorLeft == value.Length)
                            {
                                values[editing] += info.KeyChar;
                            }
                            else
                            {
                                string start = value.Substring(0, cursorLeft);
                                string end = value.Substring(cursorLeft, value.Length - cursorLeft);

                                value = start + info.KeyChar + end;
                                values[editing] = value;
                            }

                            cursorLeft++;
                        }

                        break;

                    #endregion
                }

            } while (info.Key != ConsoleKey.Enter);

            Console.CursorVisible = false;
            Console.ResetColor();
            return values;
        }

        private static bool CheckLines(string[] lines, int min, int max)
        {
            return lines.All(line =>
            {
                if (int.TryParse(line, out int res))
                {
                    return res >= min && res <= max;
                }

                return false;
            });
        }
        
        public static int[] RequestIntBatch(string title, int batchSize, string[] prefixes = null, int min = int.MinValue, int max = int.MaxValue)
        {
            string[] lines = null;
            int cursorTop = Console.CursorTop;
                
            do
            {
                Console.SetCursorPosition(0, cursorTop);
                lines = RequestStringBatch(title, batchSize, prefixes, lines);
                
            } while (lines != null
                     && !CheckLines(lines, min, max) );

            
            if (lines == null) return null;
            

            int[] returned = new int[batchSize];
            for (int i = 0; i < batchSize; i++)
            {
                returned[i] = int.Parse(lines[i]);
            }
            
            return returned;
        }
    }
}
