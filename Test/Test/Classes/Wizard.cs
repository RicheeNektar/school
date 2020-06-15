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

            public void addTrickGuess(byte tricks)
            {
                int length = _tricks.Length;
                Array.Resize(ref _tricks, length + 1);
                _tricks[length] = new byte[] { tricks, 0 };
            }

            public void setActualTricks(byte tricks)
            {
                int index = _tricks.Length - 1;
                byte[] roundGuess = _tricks[index];
                roundGuess[1] = tricks;
            }

            private int[] GetPoints()
            {
                int[] points = new int[_tricks.Length];

                for(int i = 0; i<points.Length; i++)
                {
                    byte[] trick = _tricks[i];

                    byte guess = trick[0];
                    byte actual = trick[1];

                    int round = -1;

                    if (guess == actual)
                    {
                        round = 20 + actual * 10;
                    }
                    else
                    {
                        round = Math.Abs(actual - guess) * -10;
                    }

                    points[i] = round;
                }

                return points;
            }

            public int Correct => _tricks.Sum(trick => trick[0] == trick[1] ? 1 : 0);
            public int Wrong => _tricks.Sum(trick => trick[0] != trick[1] ? 1 : 0);

            public int[] Stats => GetPoints();


            public string Name { get; set; }
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
            
            
            private Player[] _players;
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
                        ShowStats();
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
                int[] guesses = LineEditor.RequestIntBatch("Enter Guesses", _players.Length, names, 0, 255);

                if (guesses != null)
                {
                    for (int i = 0; i < _players.Length; i++)
                    {
                        _players[i].addTrickGuess((byte) guesses[i]);
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

            public void ShowStats()
            {
                // Gather Stats
                string[] output = new string[_players.Length];
                int longestName = _players.Max(player => player.Name.Length) + 1;

                for(int i = 0; i < _players.Length; i++)
                {
                    Player player = _players[i];
                    string playerName = player.Name;
                    int[] stats = player.Stats;
                   
                    int total = 0;
                    string line = playerName
                        + new string(Enumerable.Repeat(' ', longestName - playerName.Length).ToArray()) + '\n';

                    foreach(int round in stats)
                    {
                        line += string.Format("{0}\n", round);
                        total += round;
                    }

                    output[i] = line + total;
                }

                // Format output to Table

                string[] formattedOutput = new string[output.Length];
                for(int i = 0; i<output.Length; i++)
                {
                    string[] lines = output[i].Split('\n');

                    int longestLine = lines.Max(line1 => line1.Length);
                    int longestPoint = lines.Max(line1 =>
                    {
                        if (int.TryParse(line1.Replace("\n", ""), out int length))
                        {
                            return length;
                        }
                        return -1;
                    });

                    string statLine = lines[0] + new string(new char[longestLine - lines[0].Length).Replace('\n',);

                    for (int j = 1; j<lines.Length; j++)
                    {
                        string line = lines[i];
                        string whiteSpaces = new string(new char[longestLine - line.Length]);
                        line += "\n" + lines[j];
                    }

                    formattedOutput[i] = statLine;
                }

                foreach(string line in formattedOutput)
                {
                    Console.WriteLine(line);
                }

                Console.ReadLine();
            }
        }
    }
}
 