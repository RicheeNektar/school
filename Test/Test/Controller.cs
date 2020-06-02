using System;
using Test.Classes;
using Test.InputAPI;

namespace Test
{
    public class Controller
    {
        public static void HandleInput(int command)
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
                    Program.Terminate();
                    break;

                default:
                    throw new Exception("Unknown Command");
            }
        }

        private static void CreateGame()
        {
            int typeSelection = MultipleChoice.Show("Select game", GameTypeMethods.GetAllFullNames());
            GameType gameType = (GameType) Enum.GetValues(typeof(GameType)).GetValue(typeSelection);
            Console.WriteLine();

            int players =
                NumberInput.Show("How many players", gameType.Minimum(), gameType.Maximum());
            Console.Clear();

            string[] names = LineEditor.RequestStringBatch("Enter player names", players);
            Console.Clear();

            dynamic game = gameType.CreateGame(names);
            MainMenu(game);
        }

        private static void LoadGame()
        {
            // TODO
        }
        
        private static void MainMenu(dynamic game)
        {
            bool isRunning;
            string[] cmds = game.Commands;
            
            do
            {
                Console.Clear();
                int selected = MultipleChoice.Show("Main Menu", cmds);
                
                Console.Clear();
                isRunning = game.HandleCommand(selected);
                
            } while (isRunning);
        }
    }
}