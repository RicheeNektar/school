using System;
using System.Linq;
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

                return ""; //  return string.Format(format, Name, Points, Correct, Wrong);
            }

            public void setActualTricks(byte tricks)
            {
                int index = _tricks.Length;
                Array.Resize(ref _tricks, index + 1);

                byte[] roundGuess = new byte[2];
                roundGuess[0] = TricksGuess;
                roundGuess[1] = tricks;
                _tricks[index] = roundGuess;
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
            public byte TricksGuess { get; set; }
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
                "Save Game",
                "Back to Main Menu"
            };

            public static Game FromBytes(byte[] bytes, string fileLocation)
            {
                int rounds = bytes[0];
                int playerIndex = 0;
                Player[] players = new Player[TYPE.Maximum()];

                for (int i = 1; i < bytes.Length; i++)
                {
                    string name = "";
                    int nameLength = bytes[i++];

                    for (; i < i + nameLength; i++)
                    {
                        name += bytes[i];
                    }
                    
                    byte[][] tricks = new byte[rounds][];
                    for (; i < i + rounds;)
                    {
                        tricks[i - rounds] = new [] {
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
                        ShowCourse();
                        break;

                    case 4:
                        break; // TODO SAVE

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
                
                int[] guesses = LineEditor.RequestIntBatch("Enter Guesses", _players.Length, names, 0, 255);

                if (guesses != null)
                {
                    for (int i = 0; i < _players.Length; i++)
                    {
                        _players[i].TricksGuess = (byte) guesses[i];
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
        }
    }
}
 