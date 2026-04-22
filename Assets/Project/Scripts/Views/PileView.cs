using UnityEngine;
using System.Collections.Generic;

namespace Solitaire.Views
{
    public class PileView : MonoBehaviour
    {
        public enum PileType { Tableau, Foundation, Stock, Waste }
        public PileType Type;

        // lista visual das cartas que estão filhas desta pilha 
        public List<CardView> CardsInPile = new List<CardView>();

        public Vector3 GetNextCardPosition()
        {
            if(Type == PileType.Tableau)
            {
                // offset vertical para as cartas do tableau aparecerem cascateadas
                float yOffset = CardsInPile.Count * - 0.45f;
                return transform.position + new Vector3(0, yOffset, 0); 
            }
            return transform.position;
        }

        public void AddCard(CardView card)
        {
            CardsInPile.Add(card);
        }

        // public CardView GetLastCard()
        // {
        //     return 
        // }
    }
}
