using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Test.Classes;
using Test.InputAPI;

namespace Test
{
    public class Program
    {
        private static bool isRunning = true;
        private static readonly string[] commands = {
            "Create Game",
            "Load Game",
            "Exit"
        };

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Load();
            
            do
            {
                Console.Clear();

                int item = MultipleChoice.Show("Choose an Action", commands);
                Console.Clear();

                Controller.HandleInput(item);
            } while (isRunning);
        }

        private static void Load()
        {
            Defaults.GetColors();
        }

        public static void Terminate()
        {
            isRunning = false;
        }

        private static string GetEmpty(string whiteSpace, string roundLine)
        {
            return $"\n{whiteSpace}{Regex.Replace(roundLine, ".", " ")}";
        }
        
        public static string[] FormatMergeLines(string[] output)
            {
                string[] formattedOutput = new string[output.Length];
                for(int i = 0; i<output.Length; i++)
                {
                    string[] lines = output[i].Split('\n');
                    int longestLine = lines.Max(line => line.Length) + 1;

                    string formatted = lines[0] + new string(new char[longestLine - lines[0].Length])
                                                      .Replace('\0', ' ');

                    for (int j = 1; j < lines.Length; j++)
                    {
                        string roundLine = lines[j].Replace("\n", "");
                        int remaining = longestLine - roundLine.Length;
                        
                        string whiteSpace = new string(new char[remaining])
                            .Replace('\0', ' ');
                        
                        formatted += j + 1 == lines.Length ? GetEmpty(whiteSpace, roundLine) : "";
                        formatted += $"\n{whiteSpace}{roundLine}";
                    }
                    
                    formattedOutput[i] = formatted;
                }

                int totalLines = formattedOutput[0].Split('\n').Length;
                string[] merged = new string[totalLines];

                for (int i = 0; i < totalLines; i++)
                {
                    string line = "";
                    
                    for (int j = 0; j < formattedOutput.Length; j++)
                    {
                        line += $"{formattedOutput[j].Split('\n')[i]} | ";
                    }

                    merged[i] = line;
                }

                return merged;
            }
    }
}
