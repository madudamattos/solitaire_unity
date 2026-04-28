using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Solitaire.Core;

namespace Solitaire.Views
{
    public class PileView : MonoBehaviour
    {
        [SerializeField] public PileType Type;

        [SerializeField] public Suit Suit;

        // lista visual das cartas que estão filhas desta pilha 
        private List<CardView> CardsInPile = new List<CardView>();

        public Vector3 GetNextCardPosition(bool dealing = false)
        {
            if(Type == PileType.Tableau)
            {
                float offset = dealing ? 0.45f : 0.90f;

                // offset vertical para as cartas do tableau aparecerem cascateadas
                float yOffset = CardsInPile.Count * - offset;
                return transform.position + new Vector3(0, yOffset, 0); 
            }
            return transform.position;
        }

        public void AddCard(CardView card)
        {
            CardsInPile.Add(card);
        }

        public CardView GetLastCard()
        {
            return CardsInPile.Last();
        }

        public int GetPileCount()
        {
            return CardsInPile.Count;
        }

        public List<CardView> GetCardsInPile()
        {
            return CardsInPile;
        }

        public void RemoveCard(CardView card)
        {
            if(CardsInPile.Contains(card))
                CardsInPile.Remove(card);
        }

        public List<CardView> GetCardsFrom(CardView startCard)
        {
            int startIndex = CardsInPile.IndexOf(startCard); 

            // Retorna uma lista vazia caso a carta não seja encontrada (fallback de segurança)
            if (startIndex == -1 ) return new List<CardView>();
            
            return CardsInPile.GetRange(startIndex, CardsInPile.Count - startIndex);
        }
    }
}
