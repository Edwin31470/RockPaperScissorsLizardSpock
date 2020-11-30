using System;
using System.Collections.Generic;
using System.Linq;

namespace RockPaperScissorsLizardSpock
{
    // enum to define moves
    enum Move
    {
        none,
        rock,
        paper,
        scissors,
        lizard,
        spock
    }

    // enum to define result of a round
    enum Result
    {
        none,
        win,
        lose,
        draw
    }

    // class for setting up the program and starting the game
    class MainClass
    {
        static void Main(string[] args)
        {
            while(true)
            {
                int rounds = GetRounds();
                Console.WriteLine();

                bool advanced = GetAdvancedGame();
                Console.WriteLine();

                GameController game = new GameController(rounds, advanced);

                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("\nPress any key to play again");
                Console.ReadLine();
                Console.WriteLine();
            }
        }

        // asks the user for a number of rounds as an int (must be greater than 0 and odd [so there is always a winner])
        static int GetRounds()
        {
            Console.Write("How many rounds would you like to play? (enter an odd number greater than 0): ");

            while (true)
            {
                string input = Console.ReadLine();

                // if input is not an int, is odd or is less than 1 then repeat the while and ask the user for another choice
                if (Int32.TryParse(input, out int rounds) == false || rounds % 2 == 0 || rounds < 1)
                {
                    Console.Write("Invalid input, please enter an odd number greater than 0: ");
                }
                // else return the int input
                else
                {
                    return rounds;
                }
            }
        }

        // asks the user whether they want to play a basic or an advanced game (advanced is true)
        static bool GetAdvancedGame()
        {
            Console.Write("Would you like to play an advanced game? (Y or N): ");

            while (true)
            {
                string input = Console.ReadLine().ToLower();

                if (input == "y" || input == "yes")
                {
                    return true;
                }
                else if (input == "n" || input == "no")
                {
                    return false;
                }
                else
                {
                    Console.Write("Invalid input, please write either Y for yes or N for no: ");
                }
            }
        }
    }

    // class to handle game logic and information (turn number etc.) and actually playing the game
    class GameController
    {
        // dictionary to define which move beats which (move associated with the key beats any move in the list associated with the value)
        static Dictionary<Move, List<Move>> wins = new Dictionary<Move, List<Move>>()
        {
            { Move.rock, new List<Move>() { Move.scissors, Move.lizard } }, //e.g. rock beats both scissors and lizard
            { Move.paper, new List<Move>() { Move.rock, Move.spock } },
            { Move.scissors, new List<Move>() { Move.paper, Move.lizard } },
            { Move.lizard, new List<Move>() { Move.paper, Move.spock } },
            { Move.spock, new List<Move>() { Move.rock, Move.scissors } },
        };

        int playerWins = 0; // how many times the player has won
        int opponentWins = 0; // how many times the opponent has won
        int maxTurns = 0; // store how many rounds we are playing a best of
        int turns = 0; // store how many turns there have been
        int[] moveCounter = new int[6]; // list to store the number of times a specific move is used

        // runs a best of X game
        public GameController(int X, bool advancedGame)
        {
            maxTurns = X;

            if (advancedGame == false)
            {
                PlayBasicGame(MoveGetter.GetPlayerMoveBasic, MoveGetter.GetOpponentMoveBasic);
            }
            else
            {
                PlayBasicGame(MoveGetter.GetPlayerMoveAdvanced, MoveGetter.GetOpponentMoveAdvanced);
            }
        }

        // runs a game with only the moves rock, paper and scissors
        void PlayBasicGame(Func<Move> playerMoveGetter, Func<Move> opponentMoveGetter)
        {
            // while no player has won the game, continue playing rounds (a game win is when the other player cannot win the game even if they win all the remaining rounds)
            while (playerWins + (maxTurns - turns) > opponentWins && opponentWins + (maxTurns - turns) > playerWins)
            {
                Console.WriteLine("----------------------------------------------------------------");

                // get move choices
                Move playerMove = playerMoveGetter();
                Move opponentMove = opponentMoveGetter();

                Console.WriteLine("Your move is: {0}", playerMove.ToString());
                Console.WriteLine("Your opponent's move is: {0}", opponentMove.ToString());

                // add those moves to the counter
                moveCounter[(int)playerMove]++;
                moveCounter[(int)opponentMove]++;

                // see who wins
                Result result = CompareMoves(playerMove, opponentMove);
                Console.WriteLine();

                // if a draw, repeat the loop (a draw does not count as a turn)
                if (result == Result.draw)
                {
                    continue;
                } 

                // if there is no draw then someone wins - increment the turn and their win counter
                if (result == Result.win) { playerWins++; }
                else if (result == Result.lose) { opponentWins++; }
                turns++;

                // print the new state of the game
                Console.WriteLine("You have won {0} rounds and your opponent has won {1} rounds", playerWins, opponentWins);
                Console.WriteLine("There are {0} rounds remaining", maxTurns - turns);
                Console.WriteLine();
            }

            // end the game
            EndGame();
        }

        // prints winner, turn number etc.
        void EndGame()
        {
            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------");
            if (playerWins > opponentWins)
            {
                Console.WriteLine("Congratulations, you win!");
            }
            else if (opponentWins > playerWins)
            {
                Console.WriteLine("You lose! Better luck next time!");
            }
            Console.WriteLine();

            Console.WriteLine("The game took {0} turns.", turns);

            Console.WriteLine("The most used move was {0}, used {1} times", (Move)moveCounter.ToList().IndexOf(moveCounter.Max()), moveCounter.Max());
        }

        // compares moves and declares a winner or a draw
        static Result CompareMoves(Move humanMove, Move opponentMove)
        {
            Console.WriteLine();

            // player wins if their move beats the opponents move
            if (wins[humanMove].Contains(opponentMove))
            {
                Console.WriteLine("{0} beats {1}. You win!", humanMove, opponentMove);
                return Result.win;
            } 
            // or the opponent wins if their move beats the player
            else if (wins[opponentMove].Contains(humanMove))
            {
                Console.WriteLine("{0} beats {1}. You lose!", opponentMove, humanMove);
                return Result.lose;

            }
            // otherwise there is a draw
            else
            {
                Console.WriteLine("Nobody wins. Draw!");
                return Result.draw;
            }
        }

       
    }

    // class to handle getting moves from player and opponent
    class MoveGetter
    {
        static Random rand = new Random();

        // get a human player's input for a basic game
        public static Move GetPlayerMoveBasic()
        {
            Move move;

            // repeatedly ask the user until they input a valid move
            while (true)
            {
                Console.WriteLine("Please choose a move by entering a number:");
                Console.WriteLine("1. Rock");
                Console.WriteLine("2. Paper");
                Console.WriteLine("3. Scissors");
                Console.WriteLine();

                string choice = Console.ReadLine();
                Console.WriteLine();

                // if InputToEnum does not succeed (i.e. the input is not a valid input) then repeat the while and ask the user for another choice
                // additonally, for a basic game we do not allow lizard or spock
                if ((move = InputToEnum(choice)) == Move.none || move == Move.lizard || move == Move.spock)
                {
                    Console.WriteLine("Invalid choice, please choose a number between 1 and 3. \n");
                }
                // else return the valid move
                else
                {
                    return move;
                }
            }
        }

        // get a human player's input for a basic game
        public static Move GetPlayerMoveAdvanced()
        {
            Move move;

            while (true)
            {
                Console.WriteLine("Please choose a move by entering a number:");
                Console.WriteLine("1. Rock");
                Console.WriteLine("2. Paper");
                Console.WriteLine("3. Scissors");
                Console.WriteLine("4. Lizard");
                Console.WriteLine("5. Spock");
                Console.WriteLine();

                string choice = Console.ReadLine();
                Console.WriteLine();

                // if InputToEnum does not succeed (i.e. the input is not a valid input) then repeat the while and ask the user for another choice
                if ((move = InputToEnum(choice)) == Move.none)
                {
                    Console.WriteLine("Invalid choice, please choose a number between 1 and 5. \n");
                }
                // else return the valid move
                else
                {
                    return move;
                }
            }
        }

        // get application's move for a basic game
        public static Move GetOpponentMoveBasic()
        {
            // the most effective stategy for the opponent is to play completely randomly, assuming their opponent (the human player) is also playing completely randomly
            return (Move)rand.Next(1, 4);

        }

        // get application's move for an advanced game
        public static Move GetOpponentMoveAdvanced()
        {
            // the most effective stategy for the opponent is to play completely randomly, assuming their opponent (the human player) is also playing completely randomly
            return (Move)rand.Next(1, 6);

        }

        // is passed a string and returns a null move unless that string is in the range 1 to 5 where it otherwise returns the equivalent enum 
        static Move InputToEnum(string input)
        {
            string[] valid = { "1", "2", "3", "4", "5" };

            // if the input is one of the valid options, convert to enum and return
            if (Array.Exists(valid, element => element == input))
            {
                return (Move)Convert.ToInt32(input);
            }
            // else return a type of none
            else
            {
                return Move.none;
            }
        }


        // simple error checking functions for my purposes
        // checks that GetOpponentMoveBasic generates approximately uniformly random moves
        static void CheckMoveGeneration()
        {
            int[] list = { 0, 0, 0, 0 };

            for (int i = 0; i < 10000; i++)
            {
                list[(int)GetOpponentMoveBasic()]++;
            }

            Console.WriteLine("Rocks: {0}", list[1]);
            Console.WriteLine("Paper: {0}", list[2]);
            Console.WriteLine("Scissors: {0}", list[3]);
        }
    }
}
