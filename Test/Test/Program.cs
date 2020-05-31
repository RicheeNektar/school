using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;

namespace Test
{
    class Program
    {
        private static bool isRunning = true;
        private static string[] commands = {
            "Create Game",
            "Load Game",
            "Exit"
        };

        static void Main(string[] args)
        {
            do
            {
                Console.Clear();

                int item = MutlipleChoice.Show("Main Menu", commands);
                HandleInput(item);
            } while (isRunning);
        }

        private static void HandleInput(int command)
        {
            switch(command)
            {
                case 0:
                    CreateGame();
                    break;

                case 1:
                    LoadGame();
                    break;

                case 2:
                    isRunning = false;
                    break;

                default:
                    throw new Exception("Unknown Command");
            }
        }

        private static void CreateGame()
        {

        }

        private static void LoadGame()
        {

        }
    }
}
