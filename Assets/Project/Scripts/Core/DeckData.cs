using UnityEngine;
using System.Collections.Generic;
using Solitaire.Models;

namespace Solitaire.Core
{
    [CreateAssetMenu(fileName = "New Deck", menuName = "Solitaire/Deck")]
    public class DeckData: ScriptableObject
    {
        public List<CardBackData> cardsBack;
        public List<CardVisualData> cards;
        
    }

    [System.Serializable]
    public struct CardVisualData
    {
        public Rank rank;
        public Suit suit;
        public Sprite cardSprite;
    }

    [System.Serializable]
    public struct CardBackData
    {
        public Back color;
        public Sprite cardBackSprite;
    }

    
}