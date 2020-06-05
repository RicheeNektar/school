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

                return string.Format(format, Name, Points, Correct, Wrong);
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

            public int Correct => _tricks.Sum(trick => trick[0] == trick[1] ? 1 : 0);
            public int Wrong => _tricks.Sum(trick => trick[0] != trick[1] ? 1 : 0);
            
            public int Points => _tricks.Sum(trick =>
            {
                byte guess = trick[0];
                byte actual = trick[1];

                if(guess == actual)
                {
                    return 20 + actual * 10;
                }
                
                return Math.Abs(actual - guess) * -10;
            });

            public string Name { get; }
        }

        public class Game
        {
            private const GameType TYPE = GameType.WIZ;
            public string[] Commands { get; } =
            {
                "Edit names",
                "Enter guess tricks",
                "Enter actual tricks",
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
            
            private string[] GetNames()
            {
                string[] returned = new string[_players.Length];
                for (int i = 0; i < _players.Length; i++)
                {
                    returned[i] = _players[i].Name;
                }

                return returned;
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
                int[] actual = LineEditor.RequestIntBatch("Enter Guesses", _players.Length, names, 0, 255);

                if (actual != null)
                {
                    for (int i = 0; i < _players.Length; i++)
                    {
                        _players[i].setActualTricks((byte) actual[i]);
                    }
                }
            }
        }
    }
}
 