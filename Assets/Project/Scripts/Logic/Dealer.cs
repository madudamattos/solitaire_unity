using System;
using System.Collections.Generic;
using UnityEngine;
using Solitaire.Views;
using Solitaire.Core;

namespace Solitaire.Logic
{
    public class Dealer
    {   
        private readonly float _timeBetweenCards = 0.05f;
        private readonly float _moveDuration = 0.35f; 
        private float _currentDelay = 0f;
        public void Deal(List<CardView> cardViews, List<PileView> tableauPiles, PileView stockPile, System.Action onDealComplete = null)
        {
            _currentDelay = 0f; // reseta o timer
            int cardIndex = 0;

            cardIndex = DealToTableau(cardViews, tableauPiles);
            DealToStock(cardViews, stockPile, cardIndex, onDealComplete);
        }

        private int DealToTableau(List<CardView> cardViews, List<PileView> tableauPiles)
        {
            int index = 0;

            for(int i=0; i < tableauPiles.Count; i++)
            {
                for(int j=0; j<= i; j++)
                {
                    CardView card = cardViews[index];
                    PileView targetPile = tableauPiles[i];  
                    
                    bool isLastCardInColumn = (j == i);

                    MoveCardToPile(card, targetPile, flipOnArrival: isLastCardInColumn);

                    card.SetSortingOrder(j); // ajuste visual para as cartas mais abaixo na hierarquia serem renderizadas por cima
                    index++;
                }
            }

            return index;
        }

        private void DealToStock(List<CardView> cardViews, PileView stockPile, int startIndex, Action onDealComplete = null)
        {
            for(int i=0; i< cardViews.Count; i++)
            {
                CardView card = cardViews[startIndex];
                bool isVeryLastCard =  i == cardViews.Count - 1;

                // se for a ultima carda do baralho agenda o callback de finalizaçao
                MoveCardToPile(card, stockPile, flipOnArrival: false, onArrival: isVeryLastCard ? onDealComplete : null);
            }
        }

        private void MoveCardToPile(CardView card, PileView pile, bool flipOnArrival, Action onArrival = null)
        {
            // estado lógico (imediato)
            Vector3 targetPosition = pile.Type == PileType.Tableau? pile.GetNextCardPosition(dealing:true) : pile.transform.position;

            card.transform.SetParent(pile.transform);
            pile.AddCard(card);
            card.GetComponent<CardInteraction>().CurrentPile = pile;

            // estado visual (assincrono)
            // Montamos uma função anônima (Action) que será executada quando o DOTween terminar o voo
            Action onAnimationComplete = () =>
            {
                if(flipOnArrival) card.RequestFlip();
                onArrival?.Invoke();
            };

            // Dispara a animação 
            card.MoveTo(targetPosition, _moveDuration, _currentDelay, onAnimationComplete);

            _currentDelay += _timeBetweenCards;
        }


    }
}