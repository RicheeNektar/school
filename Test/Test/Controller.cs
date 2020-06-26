using System;
using System.IO;
using System.Security.Cryptography;
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
            int typeSelection = MultipleChoice.Show("Select game", GameTypeMethods.GetAllFullNames()) + 1;
            GameType gameType = (GameType) Enum.GetValues(typeof(GameType)).GetValue(typeSelection);
            Console.WriteLine();

            int players =
                NumberInput.Show("How many players", gameType.Minimum(), gameType.Maximum());
            Console.Clear();

            string[] names = LineEditor.RequestStringBatch("Enter player names", players);
            Console.Clear();

            if (names != null)
            {
                dynamic game = gameType.CreateGame(names);
                MainMenu(game);
            }
        }

        private static void LoadGame()
        {
            string file = LineEditor.RequestPath(GameType.NON);

            if (File.Exists(file))
            {
                using (FileStream stream = new FileStream(file, FileMode.Open))
                {
                    try {
                        byte[] typeBytes = new byte[3];
                        stream.Read(typeBytes, 0, 3);

                        byte[] gameData = new byte[(int) stream.Length - 3];
                        stream.Read(gameData, 0, gameData.Length);

                        if (GameTypeMethods.TryParse(typeBytes, out GameType gameType))
                        {
                            dynamic game = gameType.CreateGame(gameData, file);
                            MainMenu(game);
                        }
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid save file.");
                        Console.ReadLine();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An unknown error occurred. {e.Message}");
                        Console.ReadLine();
                    }
                }
            }
            else
            {
                Console.WriteLine($"File '{file}' does not exist.");
                Console.ReadLine();
            }
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