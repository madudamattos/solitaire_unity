using Solitaire.Models;
using Solitaire.Views;

namespace Solitaire.Presenters
{
    public class CardPresenter
    {
        private CardModel _model;
        private CardView _view;

        public CardPresenter(CardModel model, CardView view)
        {
            _model = model;

            _view = view;
            _view.Bind(this);

            // Subscreve aos eventos do Model para atualizar a View
            _model.OnFaceUpChanged += _view.SetFaceUp;

            _view.SetFaceUp(_model.IsFaceUp);
        }

        public void Dispose()
        {
            if(_model != null && _view != null)
            {
                _model.OnFaceUpChanged -= _view.SetFaceUp;

                _model = null;
                _view = null;
            }
        }

        public void FlipCard()
        {
            _model.Flip();
        }
    }
}