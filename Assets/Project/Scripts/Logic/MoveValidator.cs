using Solitaire.Models;
using Solitaire.Views;
using Solitaire.Core;
using System.Collections.Generic;

namespace Solitaire.Logic
{
    /// <summary>
    /// Pure logical class containing the specific ruleset for Solitaire.
    /// Evaluates if actions proposed by the player input are valid.
    /// </summary>
    public static class MoveValidator
    {
        /// <summary>
        /// Determines if a specific card can be picked up by the player.
        /// </summary>
        /// <param name="cardToDrag">The physical card the player is trying to interact with.</param>
        /// <param name="currentPile">The pile currently holding the card.</param>
        /// <returns><c>true</c> if the card is draggable; otherwise, <c>false</c>.</returns>
        public static bool CanDrag(CardView cardToDrag, PileView currentPile)
        {
            if(GameManager.Instance.CurrentState != GameState.Playing) 
                return false;

            // Cards in the stock cannot be dragged directly; they must be drawn via click 
            if(currentPile.Type == PileType.Stock)
                return false;

            // Only the topmost exposed card of the waste pile can be dragged
            if(currentPile.Type == PileType.Waste && currentPile.GetLastCard() != cardToDrag) 
                return false;

            // Face-down cards are locked
            if (!cardToDrag.Presenter.Model.IsFaceUp) 
                return false;
            
            return true;
        }

        /// <summary>
        /// Evaluates if a card (or a stack of cards) can be legally dropped onto a target pile 
        /// according to the rules of the game.
        /// </summary>
        /// <param name="cardToMove">The logical model of the bottom-most card being moved.</param>
        /// <param name="targetPile">The destination pile view.</param>
        /// <param name="draggetCardsCount">The total amount of cards being moved in this operation.</param>
        /// <returns><c>true</c> if the ruleset permits the drop; otherwise, <c>false</c>.</returns>
        public static bool IsValidMove(CardModel cardToMove, PileView targetPile, int draggetCardsCount)
        {
            // Rule 1: Dropping onto the Tableau
            if(targetPile.Type == PileType.Tableau)
            {
                if(targetPile.GetPileCount() == 0)
                {
                    // Empty tableau columns only accept Kings
                    return cardToMove.Rank == Rank.King; 
                }

                CardModel targetCard = targetPile.GetLastCard().Presenter.Model;

                // Valid tableau moves must alternate colors and be exactly one rank lower
                bool isDifferentColor = cardToMove.IsRed != targetCard.IsRed;
                bool isDescendingOrder = (int) cardToMove.Rank == (int) targetCard.Rank - 1;

                return isDifferentColor && isDescendingOrder;
            }
            // Rule 2: Dropping onto the Foundation
            else if(targetPile.Type == PileType.Foundation)
            {
                // Foundation piles only accept one card at a time
                if(draggetCardsCount > 1) return false;

                if(targetPile.GetPileCount() == 0)
                {
                    // Empty foundation piles only accept Aces matching the pile's defined suit
                    return cardToMove.Rank == Rank.Ace && cardToMove.Suit == targetPile.Suit; 
                }

                // Protects against array out-of-bounds if the pile is already complete (13 cards)
                if(targetPile.GetPileCount() >= System.Enum.GetValues(typeof(Rank)).Length) 
                {
                    return false; 
                }

                CardModel targetCard = targetPile.GetLastCard().Presenter.Model;

                // Valid foundation moves must match the suit exactly and be one rank higher
                bool isSameSuit = cardToMove.Suit == targetPile.Suit;
                bool isAscendingOrder = (int) cardToMove.Rank == (int) targetCard.Rank + 1;

                return isSameSuit && isAscendingOrder;
            }

            return false;
        }

        /// <summary>
        /// Automatically scans all Foundation piles to check if a specific card can be legally sent to any of them.
        /// Used by the Auto-Complete routine and potentially for double-click quick moves.
        /// </summary>
        /// <param name="cardModel">The logical card being evaluated.</param>
        /// <param name="foundations">The list containing all four Foundation piles.</param>
        /// <returns>Returns the valid <c>PileView</c> if found; otherwise, returns <c>null</c>.</returns>
        public static PileView GetValidFoundation(CardModel cardModel, List<PileView> foundations)
        {
            foreach (var foundation in foundations)
            {
                if (IsValidMove(cardModel, foundation, 1))
                {
                    return foundation;
                }
            }
            return null;
        }
    }
}