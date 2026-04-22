using System;

namespace Solitaire.Models
{
    public enum Suit { Hearts, Clubs, Diamonds, Spades }
    public enum Back { Yellow, Blue, Green, Purple }
    public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King  }   

    public class CardModel
    {
        public Rank Rank { get; private set; }
        public Suit Suit { get; private set; }
        public bool IsFaceUp { get; set; }

        // Evento para que view saiba que a carta virou
        public event Action<bool> OnFaceUpChanged;

        public CardModel(Rank rank, Suit suit, bool isFaceUp = false)
        {
            Rank = rank;
            Suit = suit;
            IsFaceUp = isFaceUp;
        } 

        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
            OnFaceUpChanged?.Invoke(IsFaceUp); 
        }

        public bool IsRed => Suit == Suit.Hearts || Suit == Suit.Diamonds;
    }  
}
