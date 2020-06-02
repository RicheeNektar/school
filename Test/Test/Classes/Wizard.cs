using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.InputAPI;

namespace Test.Classes
{
    class Wizard
    {
        public class Player
        {
            private byte[][] _guesses = { };

            public Player(string name)
            {
                Name = name;
            }

            public void addTrickGuess(byte tricks)
            {
                int length = _guesses.Length;
                Array.Resize(ref _guesses, length + 1);
                _guesses[length] = new byte[] { tricks, 0 };
            }

            public void setActualTricks(byte tricks)
            {
                int index = _guesses.Length - 1;
                byte[] roundGuess = _guesses[index];
                roundGuess[1] = tricks;
            }

            public int Points => _guesses.Sum(roundGuess =>
            {
                byte guess = roundGuess[0];
                byte actual = roundGuess[1];

                if(guess == actual)
                {
                    return 20 + actual * 10;
                }
                else
                {
                    return Math.Abs(actual - guess) * -10;
                }
            });

            public string Name { get; }
        }

        public class Game
        {
            private const GameType TYPE = GameType.WIZ;
            private Player[] _players;

            public Game(string[] names)
            {
                int length = names.Length;
                _players = new Player[length];

                for(int i = 0; i<length; i++)
                {
                    _players[i] = new Player(names[i]);
                }
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

            public string[] Commands { get; } =
            {
                "Enter guess tricks",
                "Enter actual tricks",
                "Save Game",
                "Back to Main Menu"
            };
        }
    }
}
 