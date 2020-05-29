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
        public static byte[] ToByteArray(this GameType type)
        {
            return Encoding.UTF8.GetBytes(type.ToString());
        }
    }
}
