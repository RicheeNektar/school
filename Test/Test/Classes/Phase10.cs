using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;

namespace Test
{
    class Phase10
    {
        public class Player
        {
            private byte[] _points = { };
            private string _name;

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

            public int Points => _points.Sum(i => i);
            public int Phase => _points.Sum(i => i < 50 ? 1 : 0);
            public int Wins => _points.Sum(i => i == 0 ? 1 : 0);
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
        }
    }
}
