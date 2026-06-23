using System.Linq;
using System.Collections.Generic;
using Solitaire.Models;
using Solitaire.Core;

namespace Solitaire.Logic
{
    /// <summary>
    /// Static class responsible for generating the logical Deck model, containing all suits and card ranks.
    /// </summary>
    public static class DeckGenerator
    {
        /// <summary>n
        /// Creates the logical deck model.
        /// </summary>
        /// <returns> Returns the logical deck as a <c>List&lt;CardModel&gt;</c> shuffled using the Fisher-Yates method. </returns>
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
            System.Random rng = new System.Random();
            return deck.OrderBy(a => rng.Next()).ToList();
        }
    }
}