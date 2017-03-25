using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace tictactoe
{
    class Program
    {
        public static class Player
        {
            public static string One = "X";
            public static string Two = "O";
        }

        public static class Valid
        {
            public static Regex Letters = new Regex("[QWEASDZXC]", RegexOptions.IgnoreCase);
            public static Regex Numbers = new Regex("[1-9]");
            public static Regex Help = new Regex(@"[\?H]", RegexOptions.IgnoreCase);
            public static Regex Quit = new Regex("[K]", RegexOptions.IgnoreCase);

            public static bool Any(string input)
            {
                return Letters.IsMatch(input) || Numbers.IsMatch(input);
            }
        }


        static List<List<string>> board;

        static Dictionary<int, (int row, int col)> inputLookup = new Dictionary<int, (int row, int col)>();
        static char[] inputLetter = new char[] {'_','Q','W','E','A','S','D','Z','X','C' };

        static void Init()
        {
            for(var i = 0; i < 9; i++)
            {
                inputLookup.Add(i + 1, (i / 3, i % 3));
            }

            // initialize board 
            board = new List<List<string>>();
            
            for (var i = 0; i < 3; i++)
            {
                board.Add(new List<string>());
                for(var j = 0; j < 3; j++)
                {
                    board[i].Add(null);
                }
            }
        }
        

        static (int row, int col) MapInput(string input)
        {
            
            if (Valid.Letters.IsMatch(input))
            {
                char upperInput = input.ToUpper().ToCharArray()[0];
                return inputLookup[Array.IndexOf(inputLetter, upperInput) ];
            }

            if (Valid.Numbers.IsMatch(input))
            {
                return inputLookup[int.Parse(input)];
            }
            throw new ArgumentException("Invalid Input");
        }

        static void ResetBoard()
        {
            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 3; j++)
                    board[i][j] = null;
        }

        static void PrintBoard()
        {
            Console.Clear();
            foreach(var row in board)
            {
                Console.WriteLine(" {0}  {1}  {2} ", row[0] ?? ".", row[1] ?? ".", row[2] ?? ".");
            }
        }

        static string GetValidResponse(string question, string def = "")
        {
            var valid = false;
            var input = "";
            do
            {
                Console.Write(question + " ");
                input = Console.ReadLine();
                valid = Valid.Any(input) || (def != "" && input == "");

                if (Valid.Quit.IsMatch(input)) EndGame();

                if (!valid) PrintHelp();

            } while (!valid);

            return string.IsNullOrEmpty(input) ? def : input.ToUpper();

        }

        static bool WinRows(string player)
        {
            return board.Any(row => row.All(position => position == player));
        }
        static bool WinCols(string player)
        {
            for(var col = 0; col<3; col++)
            {
                if (board.Select(row => row[col]).All(position => position == player)) return true;
            }
            return false; 
        }
        static bool WinCross(string player)
        {
            return string.Format("{0}{1}{2}", board[0][0], board[1][1], board[2][2]) == player + player + player ||
                   string.Format("{0}{1}{2}", board[0][2], board[1][1], board[2][0]) == player + player + player;

        }

        static bool Win(string player)
        {
            return WinRows(player) || WinCols(player) || WinCross(player);
        }

        static bool AnyEmptyPositions()
        {
            return board.Any(row => row.Any( position => string.IsNullOrEmpty(position)));
        }


        static void GetInput(string currentPlayer)
        {
            var valid = false;
            do
            {
                var input = GetValidResponse(string.Format("Your turn player {0}", currentPlayer));
                var position = MapInput(input);

                if ( !string.IsNullOrWhiteSpace( board[position.row][position.col] ))
                    Console.WriteLine("That position is already filled. Please Try Again.");
                else
                {
                    board[position.row][position.col] = currentPlayer;
                    valid = true;
                }
            } while (!valid);

        }


        static void EndGame()
        {
            Console.WriteLine("Game Over");
            System.Environment.Exit(0);

        }

        static void PrintHelp()
        {
            Console.WriteLine("Use the following keys or 'K' to exit: ");
            for(var i = 1; i < 10; i++)
            {
                Console.Write(" {0} ", i);
                if (i % 3 == 0) Console.Write("\n");
            }
        }
        static void PrintWelcome()
        {
            Console.WriteLine("Welcome to Tic-Tac-Toe");
        }

        static string SwitchPlayer(string currentPlayer)
        {
            if (currentPlayer == Player.One)
                return Player.Two;
            return Player.One;
        }




        static void PlayTicTacToe()
        {
            string currentPlayer = Player.One;
            PrintWelcome();
            PrintBoard();
            while (true)
            {
                GetInput(currentPlayer);
                PrintBoard();
                var (won, tied) = (Win(currentPlayer), !AnyEmptyPositions());
                if (won || tied)
                {
                    var message = won ? string.Format("You win Player {0}!",currentPlayer) : "Game Tied";
                    var playAgain = "Play Again? Y/n ";
                    Console.WriteLine(message);
                    Console.Write(playAgain);
                    var input = Console.ReadLine().ToUpper();
                    if (input == "N") EndGame();

                    ResetBoard();
                    PrintBoard();
                }

                currentPlayer = SwitchPlayer(currentPlayer);


            }
        }

        static void Main(string[] args)
        {
            Init();
            PlayTicTacToe();
        }


    }
}