using Solitaire.Views;
using System.Collections.Generic;
using System.Linq;

namespace Solitaire.Logic
{
    /// <summary>
    /// Logical utility class that evaluates the current board state to determine 
    /// if victory conditions or auto-complete thresholds have been met.
    /// </summary>
    public static class WinValidator
    {
        /// <summary>
        /// Evaluates if the game has been fully solved.
        /// </summary>
        /// <param name="foundationPiles">The list containing the four foundation pile views.</param>
        /// <returns><c>true</c> if all four foundations have exactly 13 cards; otherwise, <c>false</c>.</returns>
        public static bool CheckForVictory(List<PileView> foundationPiles)
        {
            // The game is won if all 4 foundations have exactly 13 cards
            return foundationPiles.All(pile => pile.GetPileCount() == 13);
        }

        /// <summary>
        /// Determines if the board is mathematically won and can be safely auto-completed. 
        /// This happens when there are no hidden cards left in the Tableau and the Stock is empty.
        /// </summary>
        /// <param name="tableauPiles">The list of tableau pile views.</param>
        /// <param name="stockPile">The stock pile view.</param>
        /// <param name="wastePile">The waste pile view.</param>
        /// <returns><c>true</c> if the auto-complete condition is met; otherwise, <c>false</c>.</returns>
        public static bool CanAutoComplete(List<PileView> tableauPiles, PileView stockPile, PileView wastePile)
        {
            if (stockPile.GetPileCount() > 0 || wastePile.GetPileCount() > 3)
                return false;

            foreach (PileView pile in tableauPiles)
            {
                foreach (CardView card in pile.GetCardsInPile())
                {
                    if (!card.Presenter.Model.IsFaceUp)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}