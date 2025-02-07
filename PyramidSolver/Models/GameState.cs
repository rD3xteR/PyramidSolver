using PyramidSolver;
using PyramidSolver.Models;

public class GameState
{
    public List<PyramidRow> PyramidRows { get; }
    public List<Card> StockCards { get; }
    public int StockIndex { get; set; }
    public int StockIndexA { get; set; }
    public int StockIndexB { get; set; }
    public List<string> Moves { get; }

    public GameState(
        List<PyramidRow> pyramidRows,
        List<Card> stockCards,
        int stockIndex,
        int stockIndexA,
        int stockIndexB,
        List<string>? moves = null)
    {
        PyramidRows = pyramidRows;
        StockCards = stockCards;
        StockIndex = stockIndex;
        StockIndexA = stockIndexA;
        StockIndexB = stockIndexB;
        Moves = moves ?? new List<string>();
    }

    public GameState ApplyMove(PossibleMove move)
    {
        // Глубокое копирование состояния
        var newPyramid = ClonePyramidRows(PyramidRows);
        var newStock = CloneStockCards(StockCards);
        var newMoves = new List<string>(Moves);

        int newStockIndex = StockIndex;
        int newStockIndexA = StockIndexA;
        int newStockIndexB = StockIndexB;

        switch (move.MoveType)
        {
            case MoveType.MoveStock:
                newMoves.Add(MoveString.FlipStock());

                if (StockIndexA >= newStock.Where(c => c.OnDesk).Count())
                {
                    newStockIndex++;
                    newStockIndexA = 0;
                    newStockIndexB = -1;
                }
                else
                {
                    newStockIndexA++;
                    newStockIndexB++;
                }
                break;

            case MoveType.Match:
                if (GetRemovableCardInBStock(move.FirstCardToMatch!, newStock, newStockIndexB) || GetRemovableCardInBStock(move.SecondCardToMatch!, newStock, newStockIndexB))
                {
                    newStockIndexA--;
                    newStockIndexB--;
                }
                ApplyCardRemoval(newPyramid, newStock, move.FirstCardToMatch!);
                ApplyCardRemoval(newPyramid, newStock, move.SecondCardToMatch!);
                newMoves.Add(MoveString.Match(move.FirstCardToMatch!, move.SecondCardToMatch!));
                break;

            case MoveType.RemoveKing:
                if (GetRemovableCardInBStock(move.FirstCardToMatch!, newStock, newStockIndexB))
                {
                    newStockIndexA--;
                    newStockIndexB--;
                }
                ApplyCardRemoval(newPyramid, newStock, move.FirstCardToMatch!);
                newMoves.Add(MoveString.RemoveKing(move.FirstCardToMatch!));
                break;
        }

        return new GameState(
            newPyramid,
            newStock,
            newStockIndex,
            newStockIndexA,
            newStockIndexB,
            newMoves
        );
    }

    private void ApplyCardRemoval(List<PyramidRow> pyramid, List<Card> stock, Card card)
    {
        foreach (var row in pyramid)
        {
            var pyramidCard = row.RowCards.FirstOrDefault(c => c.Equals(card));
            if (pyramidCard is not null)
            {
                pyramidCard.OnDesk = false;
                break;
            }
        }

        var targetCard = stock.FirstOrDefault(c => c.Equals(card));
        if (targetCard is null) return;

        targetCard.OnDesk = false;
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

    private bool GetRemovableCardInBStock(Card cardToCheck, List<Card> cards, int stockIndexB)
    {
        if (stockIndexB < 0) return false;
        return cards.Where(c => c.OnDesk).ToList()[stockIndexB].Equals(cardToCheck);
    }
}
