using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Morser
{
    public class Program
    {
        private const int FREQUENCY = 46; //360;
        
        private static int wpm = 16;
        private static float unitLen = 1200.0f / wpm;
        private static int oneUnit = (int) unitLen;
        private static int tripleUnit = oneUnit * 3;
        private static int spaceUnit = oneUnit * 7;
        
        private static bool running = true;
        private static readonly Dictionary<char, string> morseMap = new Dictionary<char, string>();
        private static readonly string[] history = new string[10];
        
        private static Audio shortBeep = new Audio() { Duration = oneUnit, Frequency = FREQUENCY };
        private static Audio longBeep = new Audio() { Duration = tripleUnit, Frequency = FREQUENCY };

        private static void LoadMap()
        {
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Morser.map.map");

            byte[] buff = new byte[1024];
            string read = "";
            int len;
            
            while ((len = s.Read(buff,0,buff.Length)) > 0)
            {
                read += Encoding.UTF8.GetString(buff, 0, len);
            }

            foreach (string line in read.Split('\n'))
            {
                string[] split = line.Split('=');
                if (split.Length > 2)
                {
                    morseMap.Add('=', split[2]);
                }
                else
                {
                    morseMap.Add(split[0][0], split[1]);
                }
            }
        }

        private static void BeepMorse(string morse)
        {
            char prev = ' ';
            foreach (char c in morse)
            {
                if (c == ' ')
                {
                    if (prev == ' ')
                    {
                        Thread.Sleep(spaceUnit);
                    }
                    else
                    {
                        Thread.Sleep(tripleUnit);
                    }
                }
                else
                {
                    if (c == '.')
                    {
                        shortBeep.Play();
                    }
                    else
                    {
                        longBeep.Play();
                    }

                    Thread.Sleep(oneUnit);
                }

                prev = c;
            }
        }

        private static string ToMorse(string text)
        {
            string morse = "";

            foreach (char c in text)
            {
                char c1 = char.ToLower(c);
                if (morseMap.ContainsKey(c1))
                {
                    morse += morseMap[c1] + (c==' ' ? "" : " ");
                }
                else
                {
                    Console.WriteLine("Invalid key: " + c);
                    return null;
                }
            }

            return morse;
        }

        private static string FromMorse(string morse)
        {
            string text = "";
            string code = "";
            
            foreach (char c in morse)
            {
                if (c == ' ')
                {
                    if (code.Equals(""))
                    {
                        text += " ";
                    }
                    else
                    {
                        text += morseMap.First(pair => pair.Value == code).Key;
                        code = "";
                    }
                }
                else code += c;
            }

            if (!string.IsNullOrEmpty(code))
            {
                text += morseMap.First(pair => pair.Value == code).Key;
            }

            return text;
        }

        private static void PushHistory(string text)
        {
            for (int i = 8; i >= 0; i--)
            {
                history[i + 1] = history[i];
            }

            history[0] = text;
        }
        
        private static bool ValidChar(char c)
        {
            return morseMap.ContainsKey(char.ToLower(c));
        }

        private static void HandleInput(string text)
        {
            if (text.Equals("quit"))
            {
                CleanUpAudio();
                running = false;
            }
            else if (text.All(c => c==' '|| c=='.' || c=='-'))
            {
                PushHistory(text);
                Console.WriteLine(FromMorse(text));
                BeepMorse(text);
            }
            else if (text.StartsWith("wpm"))
            {
                if (text.Equals("wpm"))
                {
                    Console.WriteLine("Words per minute: " + wpm);
                }
                else
                {
                    if (int.TryParse(text.Split(' ')[1], out int nwpm))
                    {
                        wpm = nwpm;
                        unitLen = 1200f / wpm;
                        oneUnit = (int) unitLen;
                        
                        tripleUnit = oneUnit * 3;
                        spaceUnit = oneUnit * 7;

                        shortBeep.Duration = oneUnit;
                        longBeep.Duration = tripleUnit;
                    }
                }
            }
            else
            {
                PushHistory(text);
                string morse = ToMorse(text);
                    
                Console.WriteLine(morse);
                BeepMorse(morse);
            }
        }

        private static void CleanUpAudio()
        {
            shortBeep.Dispose();
            longBeep.Dispose();
        }
        
        public static void Main(string[] args)
        {
            LoadMap();

            Console.CancelKeyPress += (sender, args1) => CleanUpAudio();

            shortBeep.Write();
            longBeep.Write();
            
            string text = "";
            int histSelect = -1;
            
            Console.WriteLine("wpm <wpm> -> sets words per minute");
            Console.WriteLine("quit -> exit");

            do
            {
                if (histSelect > -1)
                {
                    string sel = history[histSelect];
                    if (!text.Equals(sel))
                    {
                        int toClear = text.Length - sel.Length;
                        if(toClear > 0) 
                            Console.Write(new string(new char[toClear]).Replace('\0', '\b'));
                    }
                    text = history[histSelect];
                }
                
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Enter Text: " + text);
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        histSelect++;
                        if (histSelect > 9) histSelect = 9;
                        if (string.IsNullOrEmpty(history[histSelect])) histSelect--;
                        break;
                    
                    case ConsoleKey.DownArrow:
                        histSelect--;
                        if (histSelect < 0)
                        {
                            Console.Write(new string(new char[text.Length]).Replace('\0', '\b'));
                            text = "";
                            if (histSelect < -1) histSelect = -1;
                        }
                        break;
                    
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        HandleInput(text);
                        text = "";
                        histSelect = -1;
                        break;
                    
                    case ConsoleKey.Backspace:
                        if (text.Length > 0)
                        {
                            text = text.Substring(0, text.Length - 1);
                            Console.Write("\b");
                            histSelect = -1;
                        }

                        break;
                    
                    case ConsoleKey.Escape:
                        text = "";
                        histSelect = -1;
                        break;
                    
                    default:
                        char press = key.KeyChar;
                        if (ValidChar(press))
                        {
                            text += key.KeyChar;
                            histSelect = -1;
                        }

                        break;
                }
            } while (running);
        }
    }
}