using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Test.Classes;
using Test.InputAPI;

namespace Test
{
    class Phase10
    {
        private class Player
        {
            private byte[] _points = { };
            private readonly string _name;

            public Player(string name)
            {
                _name = name;
            }

            public void add(int points)
            {
                if (points > -1 && points < 256)
                {
                    int len = _points.Length;
                    Array.Resize(ref _points, len + 1);
                    _points[len] = (byte)points;
                }
            }

            public override string ToString()
            {
                string format = "{0}:\nPoints : {1}\nPhase : {2}\nWins : {3}";
                return string.Format(format, _name, Points, Phase, Wins);
            }
            
            public int Points => _points.Sum(i => i);
            public int Phase => _points.Sum(i => i < 50 ? 1 : 0);
            public int Wins => _points.Sum(i => i == 0 ? 1 : 0);
            public string Name => _name;
        }

        public class Game
        {
            private const GameType TYPE = GameType.P10;
            private Player[] _players;

            public Game(string[] names)
            {
                _players = new Player[names.Length];

                for(int i = 0; i<names.Length; i++)
                {
                    _players[i] = new Player(names[i]);
                }
            }
            
            public bool HandleCommand(int choice)
            {
                switch (choice)
                {
                    case 0:
                        RoundOver();
                        break;
                    
                    case 1:
                        Stats();
                        break;
                    
                    case 4:
                        return false;
                }
                return true;
            }
            
            private string[] GetNames()
            {
                string[] returned = new string[_players.Length];
                
                for (int i = 0; i < _players.Length; i++)
                {
                    returned[i] = _players[i].Name;
                }

                return returned;
            }

            private void RoundOver()
            {
                int[] points =
                    LineEditor.RequestIntBatch("Enter points", _players.Length, GetNames(), 0, 255);
                
                for (int i = 0; i < _players.Length; i++)
                {
                    _players[i].add((byte) points[i]);
                }
            }

            private void Stats()
            {
                int spacing = 3;
                string[] output = new string[4];

                foreach (Player p in _players)
                {
                    string[] lines = p.ToString().Split('\n');
                    int longest = lines.Max(line => line.Length) + spacing;
                    
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        string whiteSpace = new string(new char[longest - line.Length]);
                        
                        if (i > 0)
                        {
                            whiteSpace = whiteSpace.Replace('\0', ' ');
                        }
                        
                        output[i] += line + whiteSpace;
                    }
                }

                Console.ResetColor();
                foreach (char c in output[0])
                {
                    if (c == '\0')
                    {
                        Console.ResetColor();
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.ForegroundColor = Defaults.Background;
                        Console.BackgroundColor = Defaults.Foreground;
                        Console.Write(c);
                    }
                }

                Console.WriteLine();
                
                for(int i = 1; i<output.Length; i++)
                {
                    Console.WriteLine(output[i]);
                }
                
                Console.ReadLine();
                Console.Clear();
            }

            private void Course()
            {
                
            }

            private void Save()
            {
                
            }

            public string[] Commands { get; } =
            {
                "Enter points",
                "Show stats",
                "Show course",
                "Save stats",
                "Back to main menu"
            };
        }
    }
}
