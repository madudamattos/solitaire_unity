using Solitaire.Views;
using System.Collections.Generic;
using System.Linq;

namespace Solitaire.Logic
{
    public static class WinValidator
    {
        
        public static bool CheckForVictory(List<PileView> foundationPiles)
        {
            // O jogo é vencido se todas as 4 foundations tiverem exatamente 13 cartas
            return foundationPiles.All(pile => pile.GetPileCount() == 13);
        }

        public static bool CanAutoComplete(List<PileView> tableauPiles, PileView stockPile, PileView wastePile)
        {
            if(stockPile.GetPileCount() > 0 || wastePile.GetPileCount() > 3 )
                return false;

            foreach(PileView pile in tableauPiles)
            {
                foreach(CardView card in pile.GetCardsInPile())
                {
                    if(!card.Presenter.Model.IsFaceUp)
                    {
                        if(!card.Presenter.Model.IsFaceUp)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}