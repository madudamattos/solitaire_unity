using System.Linq;
using Solitaire.Models;
using System.Collections.Generic;
using Solitaire.Models;

namespace Solitaire.Logic
{
    public static class DeckGenerator
    {
        public static List<CardModel> CreateFullDeck()
        {
            List<CardModel> newDeck = new List<CardModel>();

            foreach(Suit s in System.Enum.GetValues(typeof(Suit)))
            {
                foreach(Rank r in System.Enum.GetValues(typeof(Rank)))
                {
                    newDeck.Add(new CardModel(r, s));
                }
            }

            return Shuffle(newDeck);
        }

         private static List<CardModel> Shuffle(List<CardModel> deck)
        {   
            // Algoritmo de Fisher-Yates para embaralhar proffisionalmente
            System.Random rng = new System.Random();
            return deck.OrderBy(a => rng.Next()).ToList();
        }
    }
}