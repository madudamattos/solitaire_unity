using System.Collections.Generic;
using UnityEngine;
using Solitaire.Views;

namespace Solitaire.Logic
{
    public class Dealer
    {
        public void Deal(List<CardView> cardViews, List<PileView> tableauPiles, PileView stockPile)
        {
            int cardIndex = 0; 

            // distribui para o tableu
            for(int i=0; i < tableauPiles.Count; i++)
            {
                for(int j=0; j <= i; j++)
                {
                    CardView card = cardViews[cardIndex];
                    PileView targetPile = tableauPiles[i];

                    MoveCardToPile(card, targetPile); // move a carta logicamente e visualmente

                    // a ultima carta começa virada para cima 
                    if (j==i)
                    {
                        // aqui acessaremos o model via presenter ou direto se necessario
                    }

                    cardIndex++;
                }
            }   

            // o resto das cartas vai para o stock
            while (cardIndex < cardViews.Count)
            {
                MoveCardToPile(cardViews[cardIndex], stockPile);
                cardIndex++;
            }         
        }

        private void MoveCardToPile(CardView card, PileView pile)
        {
            // teleporta a carta para a posição na pilha;
            card.transform.position = pile.GetNextCardPosition();
            
            // organiza a hierarquia do unity
            card.transform.SetParent(pile.transform);

            // adiciona a carta à lista da pilha
            pile.AddCard(card);
        }        

    }
}