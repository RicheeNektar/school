using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Classes
{
    public enum GameType
    {
        P10,
        WIZ
    }

    public static class GameTypeMethods
    {
        private static string[] _fullNames;
        private static Dictionary<GameType, int[]> _playerRanges;

        public static byte[] ToByteArray(this GameType type)
        {
            return Encoding.UTF8.GetBytes(type.ToString());
        }

        public static string[] GetAllFullNames()
        {
            if (_fullNames == null)
            {
                Type gameType = typeof(GameType);
                string[] keys = Enum.GetNames(gameType);

                _fullNames = new string[keys.Length];

                for(int i = 0; i<keys.Length; i++)
                {
                    string key = keys[i];

                    GameType parsedType = (GameType)Enum.Parse(gameType, key);
                    _fullNames[i] = GetFullName(parsedType);
                }
            }
            return _fullNames;
        }

        public static string GetFullName(this GameType type)
        {
            switch(type)
            {
                case GameType.P10:
                    return "Phase10";

                case GameType.WIZ:
                    return "Wizard";

                default:
                    return "";
            }
        }
    }
}
