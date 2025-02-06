using System.Text;

namespace PyramidSolver.Models
{
    public class GameBoard
    {
        public List<PyramidRow> PyramidRows = [];
        public List<Card> StockCards = [];

        public void FillBoard()
        {
            for (int i = 1; i <= 7; i++)
            {
                Console.WriteLine($"Write {i} cards: ");
                var cardInputs = Console.ReadLine()?.ToUpper().Split(' ');
                var rowToAdd = new PyramidRow();

                foreach (var cardInput in cardInputs!)
                {
                    var card = Extensions.GetCardIfPossible(cardInput);
                    if (card is null) return;
                    rowToAdd.RowCards.Add(card);
                }

                PyramidRows.Add(rowToAdd);
            }
        }

        public void FillStock()
        {
            Console.WriteLine($"Write stock cards: ");
            var stockInputs = Console.ReadLine()?.ToUpper().Split(' ');
            if (stockInputs is null) return;

            foreach (var stockInput in stockInputs)
            {
                var card = Extensions.GetCardIfPossible(stockInput);
                if (card is null) continue;
                StockCards.Add(card);
            }
        }

        public void PrintDesk()
        {
            foreach (var row in PyramidRows)
            {
                var rowIndex = PyramidRows.IndexOf(row);
                StringBuilder sb = new(EmptyPlace.Multiply(6 - rowIndex));
                foreach (var card in row.RowCards)
                {
                    sb.Append(card.OnDesk ? card.ToString() : EmptyCard);
                    sb.Append(EmptyPlace);
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public bool TrySolve(out List<string>? moves, List<PyramidRow>? pyramidRows = null,
                                List<Card>? stockCards = null, int stockIndex = 0, int stockIndexA = 0, int stockIndexB = -1)
        {
            moves = new List<string>();
            pyramidRows ??= ClonePyramidRows(PyramidRows);
            stockCards ??= CloneStockCards(StockCards);

            if (stockIndex > 3) return false;
            if (!pyramidRows.Any(pr => pr.RowCards.Any(rc => rc.OnDesk))) return true;
            var possibleMoves = GetPossibleMoves(pyramidRows, stockCards, stockIndexA, stockIndexB);
            if (possibleMoves.Count == 0) return false;

            foreach (var possibleMove in possibleMoves)
            {
                if (possibleMove.MoveType is MoveType.MoveStock)
                {
                    if (stockIndexA > StockCards.Count)
                    {
                        stockIndex++;
                        stockIndexA = 0;
                        stockIndexB = -1;
                    }
                    else
                    {
                        stockIndexA++;
                        stockIndexB++;
                    }

                    moves.Add(MoveString.FlipStock());
                }
                else if (possibleMove is MatchMove matchMove)
                {
                    matchMove.FirstCardToMatch.OnDesk = false;
                    matchMove.SecondCardToMatch.OnDesk = false;
                    moves.Add(MoveString.Match(matchMove.FirstCardToMatch!, matchMove.SecondCardToMatch!));
                }

                if (TrySolve(out List<string> newMoves, ClonePyramidRows(pyramidRows), CloneStockCards(stockCards), stockIndex, stockIndexA, stockIndexB))
                {
                    moves.AddRange(newMoves);
                    return true;
                }
            }

            return true;
        }

        private List<PossibleMove> GetPossibleMoves(List<PyramidRow> pyramidRows, List<Card> stockCards, int stockIndexA, int stockIndexB)
        {
            var result = new List<PossibleMove>();

            var leftStock = stockCards.Where(sc => sc.OnDesk).ToList();
            var possibleCards = new List<Card>();

            if (stockIndexA >= 0)
                possibleCards.Add(leftStock[stockIndexA]);
            if (stockIndexB >= 0)
                possibleCards.Add(leftStock[stockIndexB]);

            foreach (var row in pyramidRows)
            {
                possibleCards.AddRange(row.RowCards.Where(rc
                    => rc.OnDesk && GetCardCanBeRemoved(rc)));
            }

            foreach (var card in possibleCards)
            {
                if (card.Rank is Enums.Rank.King)
                {
                    result.Add(new RemoveKingMove(MoveType.RemoveKing, card));
                    continue;
                }

                var matchableCard = GetMatchableCard(card, possibleCards);
                if (matchableCard is null) continue;

                var newMove = new MatchMove(MoveType.Match, card, matchableCard);
                if (result.Any(m => m.Equals(newMove))) continue;
                result.Add(newMove);
            }

            result.Add(new(MoveType.MoveStock));

            return result;
        }

        private List<PyramidRow> ClonePyramidRows(List<PyramidRow> pyramidRows)
        {
            var result = new List<PyramidRow>();
            foreach (var row in PyramidRows)
                result.Add(row.Clone());

            return result;
        }

        private List<Card> CloneStockCards(List<Card> cards)
        {
            var result = new List<Card>();
            foreach (var card in StockCards)
                result.Add(card.Clone());

            return result;
        }

        private Card? GetMatchableCard(Card card, List<Card> cards) =>
            cards.Where(c => !c.Equals(card)).FirstOrDefault(c => c.Rank == card.MatchableRank);

        private bool GetCardCanBeRemoved(Card card)
        {
            var row = PyramidRows.FirstOrDefault(pr => pr.RowCards.Contains(card));
            if (row is null) return true;
            var topRow = PyramidRows.ElementAtOrDefault(PyramidRows.IndexOf(row) + 1);
            if (topRow is null) return true;

            int cardIndex = row.RowCards.IndexOf(card);
            bool leftCardRemoved = !topRow.RowCards[cardIndex].OnDesk;
            bool rightCardRemoved = !topRow.RowCards[cardIndex + 1].OnDesk;

            return leftCardRemoved && rightCardRemoved;
        }

        public const string EmptyCard = "__";
        public const string EmptyPlace = "  ";
    }
}
