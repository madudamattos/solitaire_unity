using Solitaire.Models;
using Solitaire.Views;

namespace Solitaire.Presenters
{
    /// <summary>
    /// Card Presenter component (following MVP architecture) is the mediator class that bridges the <c>CardModel</c> and <c>CardView</c>.
    /// Handles the data flow and event subscriptions between the logical state and its visual representation.
    /// </summary>
    public class CardPresenter
    {
        private CardModel _model;
        private CardView _view;

        /// <summary>
        /// Exposes the underlying logical model associated with this presenter.
        /// </summary>
        public CardModel Model => _model;

        /// <summary>
        /// Initializes a new instance of the <c>CardPresenter</c>, binding the model to the view 
        /// and establishing the reactive event subscriptions.
        /// </summary>
        /// <param name="model">The pure logical representation of the card.</param>
        /// <param name="view">The MonoBehaviour visual component of the card.</param>
        public CardPresenter(CardModel model, CardView view)
        {
            _model = model;

            _view = view;
            _view.Bind(this);

            // Subscribes to the Model's events to update the View reactively
            _model.OnFaceUpChanged += _view.SetFaceUp;

            _view.SetFaceUp(_model.IsFaceUp);
        }

        /// <summary>
        /// Cleans up event subscriptions and removes references to prevent memory leaks 
        /// when the associated visual object is destroyed.
        /// </summary>
        public void Dispose()
        {
            if (_model != null && _view != null)
            {
                _model.OnFaceUpChanged -= _view.SetFaceUp;

                _model = null;
                _view = null;
            }
        }

        /// <summary>
        /// Requests a logical state flip on the model and updates the view's physical collider 
        /// based on the new orientation.
        /// </summary>
        public void FlipCard()
        {
            _model.Flip();
            _view.SetCollider(_model.IsFaceUp);
        }
    }
}