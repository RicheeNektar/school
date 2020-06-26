using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

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
        private static readonly Dictionary<GameType, int[]> _playerRanges = new Dictionary<GameType, int[]>()
        {
            [GameType.P10] = new [] {2, 6},
            [GameType.WIZ] = new [] {3, 6}
        };

        public static int Minimum(this GameType type)
        {
            return _playerRanges[type][0];
        }

        public static int Maximum(this GameType type)
        {
            return _playerRanges[type][1];
        }
        
        public static byte[] ToByteArray(this GameType type)
        {
            return Encoding.UTF8.GetBytes(type.ToString());
        }

        public static bool TryParse(byte[] bytes, out GameType type)
        {
            return GameType.TryParse(Encoding.UTF8.GetString(bytes), out type);
        }

        public static dynamic CreateGame(this GameType type, string[] names)
        {
            switch (type)
            {
                case GameType.P10:
                    return new Phase10.Game(names);
                
                case GameType.WIZ:
                    return new Wizard.Game(names);
                
                default:
                    return null;
            }
        }
        
        public static dynamic CreateGame(this GameType type, byte[] gameData, string fileLocation)
        {
            switch (type)
            {
                case GameType.P10:
                    return Phase10.Game.FromBytes(gameData, fileLocation);
                
                case GameType.WIZ:
                    return Wizard.Game.FromBytes(gameData, fileLocation);
                
                default:
                    return null;
            }
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

        private static string GetFullName(this GameType type)
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
