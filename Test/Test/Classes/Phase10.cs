using System;
using System.IO;
using System.Linq;
using Test.InputAPI;

namespace Test.Classes
{
    class Phase10
    {
        public class Player
        {
            private byte[] _points = { };

            public Player(string name)
            {
                Name = name;
            }

            public Player(string name, byte[] points)
            {
                Name = name;
                _points = points;
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
                return string.Format(format, Name, Points, Phase, Wins);
            }

            public byte[] PointArray => _points;
            public int Points => _points.Sum(i => i);
            public int Phase => _points.Sum(i => i < 50 ? 1 : 0);
            public int Wins => _points.Sum(i => i == 0 ? 1 : 0);
            
            public string Name { get; set; }
        }

        public class Game
        {
            private const GameType TYPE = GameType.P10;
            public string[] Commands { get; } =
            {
                "Enter points",
                "Edit names",
                "Show stats",
                "Show course",
                "Save stats",
                "Back to main menu"
            };
            
            public static Game FromBytes(byte[] bytes, string fileLocation)
            {
                int rounds = bytes[0];
                int playerIndex = 0;
                Player[] players = new Player[TYPE.Maximum()];

                for (int i = 1; i < bytes.Length; i++)
                {
                    string name = "";
                    int nameLen = bytes[i++];

                    for (; i < i + nameLen; i++)
                    {
                        name += (char) bytes[i];
                    }

                    byte[] points = new byte[rounds];
                    for (; i < i + rounds; i++)
                    {
                        points[i - rounds] = bytes[i];
                    }
                    
                    Player player = new Player(name, points);
                    players[playerIndex++] = player;
                }

                Array.Resize(ref players, playerIndex);
                return new Game(players, fileLocation);
            }
            

            private Player[] _players;
            private string _saveLocation;

            public Game(string[] names)
            {
                _players = new Player[names.Length];

                for(int i = 0; i<names.Length; i++)
                {
                    _players[i] = new Player(names[i]);
                }
            }

            public Game(Player[] players, string saveLocation)
            {
                _saveLocation = saveLocation;
                _players = players;
            }

            public bool HandleCommand(int choice)
            {
                switch (choice)
                {
                    case 0:
                        RoundOver();
                        break;
                    
                    case 1:
                        UpdateNames();
                        break;
                    
                    case 2:
                        RenderStats();
                        break;
                    
                    case 3:
                        ShowStats();
                        break;
                    
                    case 4:
                        Save();
                        break;
                    
                    case 5:
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

                if (points != null)
                {
                    for (int i = 0; i < _players.Length; i++)
                    {
                        _players[i].add((byte) points[i]);
                    }
                }
            }

            private void UpdateNames()
            {
                string[] names = GetNames();
                string[] newNames = LineEditor.RequestStringBatch("Change names", names.Length, null, names);

                for (int i = 0; i < names.Length; i++)
                {
                    _players[i].Name = newNames[i];
                }
            }
            
            private void RenderStats()
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

            private string[] GetPlayersCourse()
            {
                string[] output = new string[_players.Length];
                int longestName = _players.Max(plr => plr.Name.Length) + 1;
                
                for (int i = 0; i < _players.Length; i++)
                {
                    Player player = _players[i];
                    string stats = player.Name + '\n';
                    
                    foreach(byte points in player.PointArray)
                    {
                        stats += $"{points.ToString()}\n";
                    }

                    output[i] = stats + player.Points;
                }

                return output;
            }

            private void ShowStats()
            {
                string[] output = GetPlayersCourse();
                string[] formatted = Program.FormatMergeLines(output);

                foreach (string line in formatted)
                {
                    Console.WriteLine(line);
                }

                Console.ReadLine();
            }

            private void Save()
            {
                char sep = Path.DirectorySeparatorChar;
                
                string[] lines = LineEditor
                    .RequestStringBatch("Enter save location", 1, null, new []
                    {
                        _saveLocation
                        ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + sep 
                                                                                            + DateTime.Today 
                                                                                            + ".p10"
                    }, true);

                Console.Clear();
                if (lines != null)
                {
                    string file = lines[0];
                    string path = file.Substring(0, file.LastIndexOf(sep));
                    
                    Console.WriteLine(path);
                    Console.ReadLine();
                    
                    if (Directory.Exists(path))
                    {
                        try
                        {
                            if (File.Exists(file))
                            {
                                int selected =
                                    MultipleChoice.Show("File already exists. Overwrite?", "No", "", "Yes");

                                if (selected == 2)
                                {
                                    SaveGame(file);
                                }
                            }
                            else
                            {
                                SaveGame(file);
                            }
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("An error occured while saving.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"There is no such directory '{path}'.");
                        Console.ReadKey(true);
                    }
                }
            }

            private void SaveGame(string file)
            {
                FileStream stream = new FileStream(file, FileMode.Create);
                
                stream.Write(TYPE.ToByteArray(), 0, 3);
                
                stream.Write(new []
                {
                    (byte) _players[0].PointArray.Length
                }, 0, 1);

                foreach (Player player in _players)
                {
                    int pointsLength = player.PointArray.Length;
                    int nameLength = player.Name.Length;
                    
                    byte[] buffer = new byte[pointsLength + nameLength + 1];

                    buffer[0] = (byte) nameLength;
                    for (int i = 0; i < nameLength; i++)
                    {
                        buffer[i + 1] = (byte) player.Name[i];
                    }
                    
                    Array.Copy(player.PointArray, buffer, pointsLength);
                    stream.Write(buffer,0, buffer.Length);
                }
                
                stream.Close();
            }
        }
    }
}
