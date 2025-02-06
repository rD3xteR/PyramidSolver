//namespace PyramidSolver.Models
//{
//    public class GameState
//    {
//        public List<PyramidRow> PyramidRows { get; }
//        public List<Card> StockCards { get; }
//        public int StockIndex { get; }
//        public int StockIndexA { get; }
//        public int StockIndexB { get; }
//        public List<string> Moves { get; }

//        public GameState(List<PyramidRow> pyramidRows, List<Card> stockCards,
//            int stockIndex, int stockIndexA, int stockIndexB, List<string>? moves = null)
//        {
//            PyramidRows = pyramidRows;
//            StockCards = stockCards;
//            StockIndex = stockIndex;
//            StockIndexA = stockIndexA;
//            StockIndexB = stockIndexB;
//            Moves = moves ?? new List<string>();
//        }

//        public GameState ApplyMove(PossibleMove move)
//        {
//            var newPyramid = ClonePyramidRows(PyramidRows);
//            var newStock = CloneStockCards(StockCards);
//            var newMoves = new List<string>(Moves);

//            // Apply move logic here
//            // ...

//            return new GameState(newPyramid, newStock, newStockIndex,
//                newStockIndexA, newStockIndexB, newMoves);
//        }
//    }
//}
