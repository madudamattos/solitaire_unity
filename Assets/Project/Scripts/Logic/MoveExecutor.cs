using System.Collections.Generic;
using System.Linq;
using System;
using Solitaire.Views;
using Solitaire.Core;
using Solitaire.Input;
using UnityEngine;

namespace Solitaire.Logic
{
    /// <summary>
    /// Static class responsible for physically transferring cards between piles, 
    /// updating their parent hierarchies, and triggering the corresponding DOTween animations.
    /// </summary>
    public static class MoveExecutor
    {
        /// <summary>
        /// Global event broadcasted whenever a move finishes executing or undoing.
        /// Listened to by the GameManager to re-evaluate win conditions or auto-complete availability.
        /// </summary>
        public static event Action OnBoardStateChanged;

        /// <summary>
        /// Executes a forward (Pile change) move, updating the structural lists of the involved piles.
        /// </summary>
        /// <param name="cardsToMove">The stack of cards being moved.</param>
        /// <param name="sourcePile">The origin pile.</param>
        /// <param name="targetPile">The destination pile.</param>
        /// <returns>Returns <c>true</c> if the move forced a hidden card in the source Tableau to flip face-up.</returns>
        public static bool ExecuteMove(List<CardView> cardsToMove, PileView sourcePile, PileView targetPile)
        {
            bool flippedCard = false;
            
            bool stockToWaste = sourcePile.Type == PileType.Stock && targetPile.Type == PileType.Waste;
            bool wasteToStock = sourcePile.Type == PileType.Waste && targetPile.Type == PileType.Stock;

            // If recycling the waste back to stock, invert the list to simulate flipping the entire deck over
            List<CardView> cardsToProcess = wasteToStock ? Enumerable.Reverse(cardsToMove).ToList() : cardsToMove;

            int count = cardsToProcess.Count;
            for(int i = 0; i < count; i++)
            {
                CardView card = cardsToProcess[i];
                Vector3 targetPosition = targetPile.GetNextCardPosition();
                card.transform.SetParent(targetPile.transform);

                sourcePile.RemoveCard(card);
                targetPile.AddCard(card);

                card.SetSortingOrder(targetPile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = targetPile;

                if(stockToWaste)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => targetPile.UpdateWasteVisuals());

                } 
                else if (wasteToStock)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => sourcePile.UpdateWasteVisuals());
                }
                else
                {
                    card.MoveTo(targetPosition, 0.30f, 0);
                }
            }
            
            // Evaluates if the move uncovers a face-down card in the Tableau
            if(sourcePile.Type == PileType.Tableau && sourcePile.GetPileCount() > 0)
            {
                CardView cardLeftBehind = sourcePile.GetLastCard();
                if(cardLeftBehind.Presenter.Model.IsFaceUp == false)
                {
                    cardLeftBehind.RequestFlip();
                    flippedCard = true;    
                }
            }
            
            OnBoardStateChanged?.Invoke();

            return flippedCard;
        }

        /// <summary>
        /// Reverts a previously executed move. Restores the exact structural state and 
        /// reverses any implicit card flips.
        /// </summary>
        /// <param name="cardsToMove">The stack of cards being reverted.</param>
        /// <param name="sourcePile">The original starting pile.</param>
        /// <param name="targetPile">The pile the cards are currently returning from.</param>
        /// <param name="revertFlip">If true, the top card of the source pile is flipped face-down again.</param>
        public static void UndoMove(List<CardView> cardsToMove, PileView sourcePile, PileView targetPile, bool revertFlip)
        {
            if(revertFlip && sourcePile.GetPileCount() > 0)
            {
                sourcePile.GetLastCard().RequestFlip();
            }

            bool stockToWaste = sourcePile.Type == PileType.Stock && targetPile.Type == PileType.Waste;
            bool wasteToStock = sourcePile.Type == PileType.Waste && targetPile.Type == PileType.Stock;

            // Invert the process when undoing a stock-to-waste draw
            List<CardView> cardsToProcess = stockToWaste ? Enumerable.Reverse(cardsToMove).ToList() : cardsToMove;

            int count = cardsToProcess.Count;

            for(int i = 0; i < count; i++)
            {
                CardView card = cardsToProcess[i];
                card.transform.SetParent(sourcePile.transform);
                Vector3 targetPosition = sourcePile.GetNextCardPosition();

                targetPile.RemoveCard(card);
                sourcePile.AddCard(card);
                
                card.SetSortingOrder(sourcePile.GetPileCount());
                card.GetComponent<CardInteraction>().CurrentPile = sourcePile;

                if(stockToWaste)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => targetPile.UpdateWasteVisuals());

                } 
                else if (wasteToStock)
                {
                    card.RequestFlip();
                    card.MoveTo(targetPosition, 0.12f, 0, () => sourcePile.UpdateWasteVisuals());
                }
                else
                {
                    card.MoveTo(targetPosition, 0.30f, 0);
                }
            }
        
            OnBoardStateChanged?.Invoke();
        }
    }
}