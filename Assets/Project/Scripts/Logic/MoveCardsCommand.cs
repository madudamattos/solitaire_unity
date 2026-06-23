using System.Collections.Generic;
using Solitaire.Views;
using Solitaire.Managers;

namespace Solitaire.Logic
{
    /// <summary>
    /// Concrete implementation of the Command Pattern for card movements.
    /// Encapsulates all necessary data to execute a move and seamlessly revert it later.
    /// </summary>
    public class MoveCardsCommand : ICommand
    {
        private List<CardView> _cardsMoved;
        private PileView _sourcePile;
        private PileView _targetPile;
        
        /// <summary> Tracks whether this specific move caused a face-down card in the source Tableau to flip face-up. </summary>
        private bool _flippedSourceCard;

        /// <summary>
        /// Initializes a new command capturing the exact state before execution.
        /// </summary>
        /// <param name="cardsMoved">The list of cards involved in the operation.</param>
        /// <param name="source">The origin pile.</param>
        /// <param name="target">The destination pile.</param>
        public MoveCardsCommand(List<CardView> cardsMoved, PileView source, PileView target)
        {
            // Creates a shallow copy of the list to preserve the exact sequence of dragged cards
            _cardsMoved = new List<CardView>(cardsMoved);
            _sourcePile = source;
            _targetPile = target;
        }

        /// <summary>
        /// Delegates the physical execution of the move to the <c>MoveExecutor</c> 
        /// and records if the action triggered a card flip in the source pile.
        /// </summary>
        public void Execute()
        {
            _flippedSourceCard = MoveExecutor.ExecuteMove(_cardsMoved, _sourcePile, _targetPile);
        }

        /// <summary>
        /// Reverses the executed move by passing the cached state back to the <c>MoveExecutor</c>.
        /// </summary>
        public void Undo()
        {
            MoveExecutor.UndoMove(_cardsMoved, _sourcePile, _targetPile, _flippedSourceCard);
        }
    }
}