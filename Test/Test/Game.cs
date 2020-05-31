using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;

namespace Test
{
    class Game
    {
        public static dynamic CreateGame(GameType type, string[] names)
        {
            switch(type)
            {
                case GameType.P10:
                    return new Phase10.Game(names);

                case GameType.WIZ:
                    return new Wizard.Game(names);

                default:
                    return null;
            }
        }
    }
}
