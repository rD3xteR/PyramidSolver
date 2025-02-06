using PyramidSolver.Enums;
using PyramidSolver.Models;
using System.Text;

namespace PyramidSolver
{
    public static class Extensions
    {
        public static string Multiply(this string source, int multiplier) =>
            new StringBuilder().Insert(0, source, multiplier).ToString();

        public static Card? GetCardIfPossible(string cardInput)
        {
            try
            {
                var cardRank = (Rank)cardInput[0];
                var cardSuit = (Suit)cardInput[1];

                return new Card(cardRank, cardSuit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex.StackTrace);
                return null;
            }
        }
    }
}
