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

            // Subscreve aos eventos do Model para atualizar a View
            _model.OnFaceUpChanged += _view.SetFaceUp;

            _view.SetFaceUp(_model.IsFaceUp);
        }
    }
}