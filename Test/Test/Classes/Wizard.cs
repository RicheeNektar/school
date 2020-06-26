using System;
using System.IO;
using System.Linq;
using System.Text;
using Test.InputAPI;

namespace Test.Classes
{
    class Wizard
    {
        public class Player
        {
            private byte[][] _tricks = { };

            public Player(string name)
            {
                Name = name;
            }

            public Player(string name, byte[][] tricks)
            {
                Name = name;
                _tricks = tricks;
            }

            public override string ToString()
            {
                string format = "{0} :\nPoints : {1}\nCorrect : {2}\nWrong : {3}";
                return string.Format(format, Name, GetPoints().Sum(data => data >> 16), Correct, Wrong);
            }

            public void setActualTricks(byte tricks)
            {
                if (TricksGuess > -1)
                {
                    int index = _tricks.Length;
                    Array.Resize(ref _tricks, index + 1);

                    byte[] roundGuess = new byte[2];
                    roundGuess[0] = (byte) TricksGuess;
                    roundGuess[1] = tricks;
                    _tricks[index] = roundGuess;

                    TricksGuess = -1;
                }
            }

            private int[] GetPoints()
            {
                int[] points = new int[_tricks.Length];

                for(int i = 0; i<points.Length; i++)
                {
                    byte[] trick = _tricks[i];

                    byte guess = trick[0];
                    byte actual = trick[1];

                    short round;

                    if (guess == actual)
                    {
                        round = (short) (20 + actual * 10);
                    }
                    else
                    {
                        round = (short) (Math.Abs(actual - guess) * -10);
                    }

                    points[i] = round << 16 | guess << 8 | actual;
                }

                return points;
            }

            public int Correct => _tricks.Sum(trick => trick[0] == trick[1] ? 1 : 0);
            public int Wrong => _tricks.Sum(trick => trick[0] != trick[1] ? 1 : 0);

            public int[] Stats => GetPoints();


            public string Name { get; set; }
            public sbyte TricksGuess { get; set; }
            public byte Rounds => (byte) _tricks.Length;
            public byte[][] Tricks => _tricks;
        }

        public class Game
        {
            private const GameType TYPE = GameType.WIZ;
            public string[] Commands { get; } =
            {
                "Edit names",
                "Enter guess tricks",
                "Enter actual tricks",
                "Show Stats",
                "Show Course",
                "Save Game",
                "Back to Main Menu"
            };

            public static Game FromBytes(byte[] bytes, string fileLocation)
            {
                int rounds = bytes[0];
                int playerIndex = 0;
                Player[] players = new Player[TYPE.Maximum()];

                for (int i = 1; i < bytes.Length;)
                {
                    string name = "";
                    int nameLength = bytes[i++];

                    int target = i + nameLength;
                    for (; i < target; i++)
                    {
                        name += (char) bytes[i];
                    }

                    target = i + rounds;
                    byte[][] tricks = new byte[rounds][];
                    for (int j = 0; i < target; j++)
                    {
                        tricks[j] = new [] {
                            bytes[i++],
                            bytes[i++]
                        };
                    }

                    Player player = new Player(name, tricks);
                    players[playerIndex++] = player;
                }
                
                Array.Resize(ref players, playerIndex);
                return new Game(players, fileLocation);
            }
            
            
            private readonly Player[] _players;
            private string _saveLocation;

            public Game(string[] names)
            {
                int length = names.Length;
                _players = new Player[length];

                for(int i = 0; i<length; i++)
                {
                    _players[i] = new Player(names[i]);
                }
            }

            public Game(Player[] players, string saveLocation)
            {
                _players = players;
                _saveLocation = saveLocation;
            }

            public bool HandleCommand(int command)
            {
                switch(command)
                {
                    case 0:
                        UpdateNames();
                        break;

                    case 1:
                        EnterGuessTricks();
                        break;

                    case 2:
                        EnterActualTricks();
                        break;

                    case 3:
                        RenderStats();
                        break;
                        
                    case 4:
                        ShowCourse();
                        break;

                    case 5:
                        Save();
                        break;

                    case 6:
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

            private void UpdateNames()
            {
                string[] names = GetNames();
                string[] newNames = LineEditor.RequestStringBatch("Change names", names.Length, null, names);

                for (int i = 0; i < names.Length; i++)
                {
                    _players[i].Name = newNames[i];
                }
            }

            public void EnterGuessTricks()
            {
                string[] names = GetNames();
                int[] entered = new int[_players.Length];
                
                for (int i = 0; i < entered.Length; i++)
                {
                    entered[i] = _players[i].TricksGuess;
                }
                
                int[] guesses = LineEditor.RequestIntBatch("Enter Guesses", _players.Length, names, 0, 127);

                if (guesses != null)
                {
                    for (int i = 0; i < _players.Length; i++)
                    {
                        _players[i].TricksGuess = (sbyte) guesses[i];
                    }
                }
            }

            public void EnterActualTricks()
            {
                string[] names = GetNames();
                int[] actual = LineEditor.RequestIntBatch("Enter Tricks", _players.Length, names, 0, 255);

                if (actual != null)
                {
                    for (int i = 0; i < _players.Length; i++)
                    {
                        _players[i].setActualTricks((byte) actual[i]);
                    }
                }
            }

            private string[] GetPlayerRounds()
            {
                string[] output = new string[_players.Length];
                int longestName = _players.Max(player => player.Name.Length) + 1;

                for(int i = 0; i < _players.Length; i++)
                {
                    Player player = _players[i];
                    string playerName = player.Name;
                    int[] stats = player.Stats;
                   
                    int total = 0;
                    string line = playerName + '\n';

                    foreach(int round in stats)
                    {
                        short points = (short) (round >> 16);
                        byte guess = (byte) (round >> 8);
                        byte actual = (byte) round;
                        
                        line += $"{points} / {guess} / {actual}\n";
                        total += points;
                    }

                    output[i] = line + total;
                }

                return output;
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
            
            public void ShowCourse()
            {
                string[] output = GetPlayerRounds();
                string[] formatted = Program.FormatMergeLines(output);

                foreach(string line in formatted)
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine("\n** Points / Guessed / Actual Tricks");

                Console.ReadLine();
            }
            
            private void Save()
            {
                string file = LineEditor.RequestPath(_saveLocation);

                if (file != null)
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
            }

            private void SaveGame(string file)
            {
                _saveLocation = file;
                
                using (FileStream stream = new FileStream(file, FileMode.Create))
                {
                    stream.Write(TYPE.ToByteArray(), 0, 3);

                    stream.Write(new[]
                    {
                        _players[0].Rounds
                    }, 0, 1);

                    foreach (Player player in _players)
                    {
                        byte nameLen = (byte) player.Name.Length;
                        stream.WriteByte(nameLen);

                        foreach (char c in player.Name)
                            stream.WriteByte((byte) c);

                        foreach (byte[] tricks in player.Tricks)
                        {
                            stream.Write(tricks, 0, 2);
                        }
                    }
                }
            }
        }
    }
}
 