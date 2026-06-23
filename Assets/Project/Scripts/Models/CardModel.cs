using System;
using Solitaire.Core;

namespace Solitaire.Models
{
    /// <summary>
    /// Card Model component (following MVP architecture) composed by pure C# data structure representing the logical state and identity of a playing card.
    /// Contains no Unity dependencies, maintaining strict separation of concerns following.
    /// </summary>
    public class CardModel
    {
        /// <summary>
        /// The logical rank of the card (e.g., Ace, Ten, King).
        /// </summary>
        public Rank Rank { get; private set; }
        
        /// <summary>
        /// The logical suit to which the card belongs (e.g., Hearts, Diamonds, Clubs or Spades). 
        /// </summary>
        public Suit Suit { get; private set; }
        
        /// <summary>
        /// Gets or sets the current orientation state of the card.
        /// </summary>
        public bool IsFaceUp { get; set; }

        /// <summary>
        /// Event triggered whenever the <c>IsFaceUp</c> state changes.
        /// Presenters listen to this event to push updates to the UI layer.
        /// </summary>
        public event Action<bool> OnFaceUpChanged;

        /// <summary>
        /// Initializes a new logical card with a specific rank, suit, and starting orientation.
        /// </summary>
        /// <param name="rank">The numerical or face value of the card.</param>
        /// <param name="suit">The suit category of the card.</param>
        /// <param name="isFaceUp">The initial orientation (defaults to face down).</param>
        public CardModel(Rank rank, Suit suit, bool isFaceUp = false)
        {
            Rank = rank;
            Suit = suit;
            IsFaceUp = isFaceUp;
        } 

        /// <summary>
        /// Toggles the orientation state of the card and invokes the state change event.
        /// </summary>
        public void Flip()
        {
            IsFaceUp = !IsFaceUp;
            OnFaceUpChanged?.Invoke(IsFaceUp); 
        }

        /// <summary>
        /// Utility property that determines if the card belongs to a red suit (Hearts or Diamonds).
        /// </summary>
        public bool IsRed => Suit == Suit.Hearts || Suit == Suit.Diamonds;
    }  
}