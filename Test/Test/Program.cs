using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Classes;
using Test.InputAPI;

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

                int item = MultipleChoice.Show("Main Menu", commands);
                Console.Clear();

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
            int selected = MultipleChoice.Show("Select Game", GameTypeMethods.GetAllFullNames());
            Console.WriteLine();

            int players = NumberInput.Show("How many Players", 2, 6, 2);
            Console.WriteLine();

            string[] names = NameEditor.RequestBatch(players);
            Console.WriteLine();
        }

        private static void LoadGame()
        {

        }
    }
}
