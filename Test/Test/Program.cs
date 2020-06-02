using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;
using Test.InputAPI;

namespace Test
{
    public class Program
    {
        private static bool isRunning = true;
        private static readonly string[] commands = {
            "Create Game",
            "Load Game",
            "Exit"
        };

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Load();
            
            do
            {
                Console.Clear();

                int item = MultipleChoice.Show("Choose an Action", commands);
                Console.Clear();

                Controller.HandleInput(item);
            } while (isRunning);
        }

        private static void Load()
        {
            Defaults.GetColors();
        }

        public static void Terminate()
        {
            isRunning = false;
        }
    }
}
