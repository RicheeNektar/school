using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Classes
{
    class Wizard
    {
        public class Player
        {
            private byte[][] _guesses = { };
            private string _name;

            public Player(string name)
            {
                _name = name;
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

            public int Points => _guesses.Sum((byte[] roundGuess) =>
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
        }
    }
}
