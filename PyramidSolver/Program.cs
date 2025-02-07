using PyramidSolver.Models;

namespace PyramidSolver
{
    public class PyramidSolver
    {
        public static void Main()
        {
            var gameBoard = new GameBoard();
            gameBoard.FillBoard();
            gameBoard.FillStock();

            if (gameBoard.CheckBoardIsCorrect(out List<Card> incorrectCards))
            {
                Console.WriteLine("Some cards are incorrect:");
                foreach (Card card in incorrectCards)
                {
                    Console.WriteLine(card);
                }
                return;
            }

            Console.WriteLine("Board checked, all cards correct.");

            gameBoard.PrintDesk();
            gameBoard.PrintStock();

            if (gameBoard.TrySolve(out var moves, out int checkedStates))
            {
                Console.WriteLine("\nSolution found! Moves:\n");
                for (int i = 0; i < moves.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {moves[i]}");
                }
            }
            else
            {
                Console.WriteLine("\nSolution not found!");
            }
            Console.WriteLine($"Checked states: {checkedStates}");
        }
    }

    public static class MoveString
    {
        public static string FlipStock() => "Move stock card.";

        public static string Match(Card card1, Card card2) =>
            $"Match {card1} and {card2}";

        public static string RemoveKing(Card king) =>
            $"Remove king {king}";
    }
}