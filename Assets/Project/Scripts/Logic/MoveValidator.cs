using Solitaire.Models;
using Solitaire.Views;
using Solitaire.Core;

namespace Solitaire.Logic
{
    public static class MoveValidator
    {
        public static bool IsValidMove(CardModel cardToMove, PileView targetPile)
        {
            // Regra 1 : Soltou a carta no tableau
            if(targetPile.Type == PileType.Tableau)
            {
                if(targetPile.GetPileCount() == 0)
                {
                    return cardToMove.Rank == Rank.King; // se o tableau esta vazio, so aceita um rei
                }

                CardModel targetCard = targetPile.GetLastCard().Presenter.Model;

                bool isDifferentColor = cardToMove.IsRed != targetCard.IsRed;
                bool isDescendingOrder = (int) cardToMove.Rank == (int) targetCard.Rank - 1;

                return isDifferentColor && isDescendingOrder;
            }
            else if(targetPile.Type == PileType.Foundation)
            {
                if(targetPile.GetPileCount() == 0)
                {
                    return cardToMove.Rank == Rank.Ace && cardToMove.Suit == targetPile.Suit; // se o tableau esta vazio, so aceita um as do nipe do tableu
                }

                if(targetPile.GetPileCount() >= System.Enum.GetValues(typeof(Rank)).Length) 
                {
                    return false; // pilha cheia não cabe mais nada
                }

                CardModel targetCard = targetPile.GetLastCard().Presenter.Model;

                bool isSameSuit = cardToMove.Suit == targetPile.Suit;
                bool isAscendingOrder = (int) cardToMove.Rank == (int) targetCard.Rank + 1;

                return isSameSuit && isAscendingOrder;
            }

            return false;
        }
    }
}