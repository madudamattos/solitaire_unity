using System;
using System.Collections.Generic;
using UnityEngine;
using Solitaire.Views;
using Solitaire.Core;
using Solitaire.Input;

namespace Solitaire.Logic
{
    /// <summary>
    /// Class responsible for the initial distribution of cards at the start of a game. 
    /// Handles the sequenced dealing animations and logical placement into the Tableau and Stock piles.
    /// </summary>
    public class Dealer
    {   
        private readonly float _timeBetweenCards = 0.05f;
        private readonly float _moveDuration = 0.35f; 
        private float _currentDelay = 0f;

        /// <summary>
        /// Initiates the dealing sequence, distributing the instantiated deck across the board.
        /// </summary>
        /// <param name="cardViews">The complete list of instantiated card views.</param>
        /// <param name="tableauPiles">The list of tableau piles to receive the initial cascading layout.</param>
        /// <param name="stockPile">The stock pile to receive all remaining cards.</param>
        /// <param name="onDealComplete">Optional callback invoked when the final card animation finishes.</param>
        public void Deal(List<CardView> cardViews, List<PileView> tableauPiles, PileView stockPile, System.Action onDealComplete = null)
        {
            // Resets the animation delay timer
            _currentDelay = 0f; 
            int cardIndex = 0;

            cardIndex = DealToTableau(cardViews, tableauPiles);
            DealToStock(cardViews, stockPile, cardIndex, onDealComplete);
        }

        /// <summary>
        /// Distributes cards into the tableau piles in a standard Solitaire staircase pattern.
        /// </summary>
        /// <param name="cardViews">The complete list of instantiated card views.</param>
        /// <param name="tableauPiles">The target tableau piles.</param>
        /// <returns>The index of the next available card in the deck after dealing to the tableau.</returns>
        private int DealToTableau(List<CardView> cardViews, List<PileView> tableauPiles)
        {
            int index = 0;

            for(int i = 0; i < tableauPiles.Count; i++)
            {
                for(int j = 0; j <= i; j++)
                {
                    CardView card = cardViews[index];
                    PileView targetPile = tableauPiles[i];  
                    
                    bool isLastCardInColumn = (j == i);

                    MoveCardToPile(card, targetPile, flipOnArrival: isLastCardInColumn);

                    // Visual adjustment to ensure cards lower in the column are rendered on top
                    card.SetSortingOrder(j); 
                    index++;
                }
            }

            return index;
        }

        /// <summary>
        /// Moves the remaining cards from the deck into the stock pile.
        /// </summary>
        /// <param name="cardViews">The complete list of instantiated card views.</param>
        /// <param name="stockPile">The target stock pile.</param>
        /// <param name="startIndex">The starting index of the remaining undealt cards.</param>
        /// <param name="onDealComplete">Optional callback invoked when the final card arrives.</param>
        private void DealToStock(List<CardView> cardViews, PileView stockPile, int startIndex, Action onDealComplete = null)
        {
            for(int i = startIndex; i < cardViews.Count; i++)
            {
                CardView card = cardViews[i];
                bool isVeryLastCard = i == cardViews.Count - 1;

                // If it is the very last card of the deck, schedules the completion callback
                MoveCardToPile(card, stockPile, flipOnArrival: false, onArrival: isVeryLastCard ? onDealComplete : null);
            }
        }

        /// <summary>
        /// Updates the logical hierarchy of a card and schedules its visual transition.
        /// </summary>
        /// <param name="card">The card to be moved.</param>
        /// <param name="pile">The target pile.</param>
        /// <param name="flipOnArrival">Whether the card should be flipped face-up upon reaching the target.</param>
        /// <param name="onArrival">Optional callback invoked when this specific card finishes its animation.</param>
        private void MoveCardToPile(CardView card, PileView pile, bool flipOnArrival, Action onArrival = null)
        {
            Vector3 targetPosition = pile.GetNextCardPosition();

            card.transform.SetParent(pile.transform);
            pile.AddCard(card);
            card.GetComponent<CardInteraction>().CurrentPile = pile;

            // Asynchronous visual state update
            // Defines an anonymous function (Action) to be executed when the DOTween animation completes
            Action onAnimationComplete = () =>
            {
                if(flipOnArrival) card.RequestFlip();
                onArrival?.Invoke();
            };

            // Triggers the animation
            card.MoveTo(targetPosition, _moveDuration, _currentDelay, onAnimationComplete);

            _currentDelay += _timeBetweenCards;
        }
    }
}