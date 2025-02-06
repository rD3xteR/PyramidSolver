﻿using PyramidSolver.Models;

namespace PyramidSolver
{
    public class PyramidSolver
    {
        public static void Main()
        {
            var gameBoard = new GameBoard();
            gameBoard.FillBoard();
            gameBoard.FillStock();
            gameBoard.PrintDesk();
            gameBoard.TrySolve(out List<string> moves);
            foreach (var move in moves)
            {
                Console.WriteLine(move);
            }
        }
    }

    public static class MoveString
    {
        public static string Match(Card cardA, Card cardB) =>
            $"Match {cardA} with {cardB}.";

        public static string FlipStock() =>
            $"Move stock card.";
    }
}