namespace Solitaire.Core
{
    /// <summary>
    /// Represents the four standard logical suits in a deck of cards.
    /// </summary>
    public enum Suit { Hearts, Clubs, Diamonds, Spades }

    /// <summary>
    /// Represents the visual color variations available for the card backs.
    /// </summary>
    public enum Back { Yellow, Blue, Green, Purple }

    /// <summary>
    /// Represents the logical rank of a card, mapped to its standard numerical value.
    /// </summary>
    public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

    /// <summary>
    /// Defines the structural role of a card pile within the Solitaire game board.
    /// </summary>
    public enum PileType 
    { 
        /// <summary> The main playing area where cards are built down in alternating colors. </summary>
        Tableau, 
        
        /// <summary> The target piles where cards are built up by suit, from Ace to King. </summary>
        Foundation, 
        
        /// <summary> The draw pile containing face-down cards not yet in play. </summary>
        Stock, 
        
        /// <summary> The discard pile holding face-up cards drawn from the Stock. </summary>
        Waste 
    }

    /// <summary>
    /// Defines the high-level states of the game's lifecycle managed by the GameManager.
    /// </summary>
    public enum GameState 
    { 
        /// <summary> Player is in the main menu or settings screen. </summary>
        Menu, 
        
        /// <summary> The initial phase where cards are being distributed to the Tableau and Stock. </summary>
        Dealing, 
        
        /// <summary> The core interactive state where the timer is running and player input is accepted. </summary>
        Playing, 
        
        /// <summary> The automated sequence that resolves the remaining board after victory conditions are met. </summary>
        AutoComplete, 
        
        /// <summary> Gameplay is temporarily halted; input and timers are frozen. </summary>
        Paused, 
        
        /// <summary> The final state after the board is fully solved, displaying the victory screen and final stats. </summary>
        GameOver 
    }
}