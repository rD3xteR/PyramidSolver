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

        public void PrintDesk(List<PyramidRow>? pyramidRows = null)
        {
            pyramidRows ??= PyramidRows;
            foreach (var row in pyramidRows)
            {
                var rowIndex = pyramidRows.IndexOf(row);
                StringBuilder sb = new(EmptyPlace.Multiply(6 - rowIndex));
                foreach (var card in row.RowCards)
                {
                    sb.Append(card.OnDesk ? card.ToString() : EmptyCard);
                    sb.Append(EmptyPlace);
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public void PrintStock(List<Card>? stock = null)
        {
            stock ??= StockCards;

            StringBuilder sb = new();
            foreach (var card in stock)
            {
                sb.Append(card.OnDesk ? card.ToString() : EmptyCard);
                sb.Append(EmptyPlace);
            }
            Console.WriteLine(sb.ToString());

        }

        public bool TrySolve(out List<string>? moves, out int checkedStatesCount)
        {
            moves = new List<string>();
            checkedStatesCount = 0;
            var stack = new Stack<GameState>();
            var initialState = new GameState(
                PyramidRows,
                StockCards,
                stockIndex: 0,
                stockIndexA: 0,
                stockIndexB: -1,
                moves: new List<string>()
            );
            stack.Push(initialState);

            var visitedStates = new HashSet<string>();

            while (stack.Count > 0)
            {
                var currentState = stack.Pop();

                var stateStockCount = currentState.StockCards.Where(c => c.OnDesk).Count();
                if (currentState.StockIndexA >= stateStockCount - 1)
                {
                    currentState.StockIndex++;
                    currentState.StockIndexA = 0;
                    currentState.StockIndexB = -1;
                }

                if (currentState.StockIndex >= 3) continue;

                // Проверка на конечное состояние
                if (!currentState.PyramidRows.Any(pr => pr.RowCards.Any(rc => rc.OnDesk)))
                {
                    moves = currentState.Moves;
                    return true;
                }

                PrintDesk(currentState.PyramidRows);
                PrintStock(currentState.StockCards);
                Console.WriteLine($"Stock params: {currentState.StockIndex}, {currentState.StockIndexA}, {currentState.StockIndexB}");

                // Проверка уникальности состояния
                var stateHash = GetStateHash(currentState);
                if (visitedStates.Contains(stateHash)) continue;
                visitedStates.Add(stateHash);

                checkedStatesCount++;

                var possibleMoves = GetPossibleMoves(
                    currentState.PyramidRows,
                    currentState.StockCards,
                    currentState.StockIndexA,
                    currentState.StockIndexB
                );

                foreach (var move in possibleMoves)
                {
                    var newState = currentState.ApplyMove(move);
                    if (newState.StockIndex >= 3) continue; // Пропускаем невалидные состояния
                    stack.Push(newState);
                }
            }

            return false;
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
                    => rc.OnDesk && GetCardCanBeRemoved(rc, pyramidRows)));
            }

            result.Add(new(MoveType.MoveStock));

            foreach (var card in possibleCards)
            {
                if (card.Rank is Enums.Rank.King)
                {
                    result.Add(new(MoveType.RemoveKing, card));
                    continue;
                }

                var matchableCard = GetMatchableCard(card, possibleCards);
                if (matchableCard is null) continue;

                var newMove = new PossibleMove(MoveType.Match, card, matchableCard);
                if (result.Any(m => m.Equals(newMove))) continue;
                result.Add(newMove);
            }

            return result;
        }

        private Card? GetMatchableCard(Card card, List<Card> cards) =>
            cards.Where(c => !c.Equals(card)).FirstOrDefault(c => c.Rank == card.MatchableRank);

        private bool GetCardCanBeRemoved(Card card, List<PyramidRow> pyramidRows)
        {
            var row = pyramidRows.FirstOrDefault(pr => pr.RowCards.Contains(card));
            if (row is null) return true;
            var topRow = pyramidRows.ElementAtOrDefault(pyramidRows.IndexOf(row) + 1);
            if (topRow is null) return true;

            int cardIndex = row.RowCards.IndexOf(card);
            bool leftCardRemoved = !topRow.RowCards[cardIndex].OnDesk;
            bool rightCardRemoved = !topRow.RowCards[cardIndex + 1].OnDesk;

            return leftCardRemoved && rightCardRemoved;
        }

        private string GetStateHash(GameState state)
        {
            var sb = new StringBuilder();

            // Хеширование пирамиды
            foreach (var row in state.PyramidRows)
            {
                foreach (var card in row.RowCards)
                {
                    sb.Append(card.OnDesk ? '1' : '0');
                }
            }

            sb.Append(state.StockIndex);
            sb.Append(state.StockIndexA);
            sb.Append(state.StockIndexB);

            // Хеширование стока
            foreach (var card in state.StockCards)
            {
                sb.Append(card.OnDesk ? '1' : '0');
            }

            return sb.ToString();
        }

        public const string EmptyCard = "__";
        public const string EmptyPlace = "  ";
    }
}
