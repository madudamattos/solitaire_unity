using UnityEngine;
using System.Collections.Generic;

namespace Solitaire.Core
{
    /// <summary>
    /// Inspector-configurable data container (ScriptableObject) responsible for mapping 
    /// the logical representations (Suit and Rank) and visual representations (sprites) for the front and back of a deck.
    /// </summary>
    [CreateAssetMenu(fileName = "New Deck", menuName = "Solitaire/Deck")]
    public class DeckData : ScriptableObject
    {   
        /// <summary>
        /// List containing the available visual variations for the card backs of this deck.
        /// </summary>
        public List<CardBackData> cardsBack;
        
        /// <summary>
        /// List containing the sprite mapping for the faces of all logical cards in the deck.
        /// </summary>
        public List<CardVisualData> cards;
    }

    /// <summary>
    /// Serializable data structure that associates a logical card (Rank and Suit) 
    /// with its respective visual rendering (Sprite).
    /// </summary>
    [System.Serializable]
    public struct CardVisualData
    {
        /// <summary>
        /// The card's rank (e.g., Ace, Ten, King).
        /// </summary>
        public Rank rank;

        /// <summary>
        /// The logical suit to which the card belongs (Hearts, Spades, Diamonds, Clubs).
        /// </summary>
        public Suit suit;

        /// <summary>
        /// Sprite of the corresponding card face.
        /// </summary>
        public Sprite cardSprite;
    }

    /// <summary>
    /// Serializable data structure that associates the selected card color
    /// with its respective sprite.
    /// </summary>
    [System.Serializable]
    public struct CardBackData
    {
        /// <summary>
        /// The logical identifier for the card's color.
        /// </summary>
        public Back color;

        /// <summary>
        /// Sprite that will be rendered when the card is face down.
        /// </summary>
        public Sprite cardBackSprite;
    }
}