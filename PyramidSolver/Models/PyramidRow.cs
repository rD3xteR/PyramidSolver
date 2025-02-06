namespace PyramidSolver.Models
{
    public class PyramidRow()
    {
        public List<Card> RowCards = [];

        public PyramidRow Clone()
        {
            var cards = new List<Card>();
            foreach (var card in RowCards)
            {
                cards.Add(card.Clone());
            }

            return new() { RowCards = cards };
        }
    }
}
