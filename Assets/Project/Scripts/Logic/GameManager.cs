using System.Linq;
using Solitaire.Models;
using Solitaire.Views;
using Solitaire.Logic;
using Solitaire.Presenters;
using System.Collections.Generic;
using UnityEngine;
using Solitaire.Core;

namespace Solitaire.Logic
{   
    public class GameManager : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private DeckData deckData;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Back cardsBack = Back.Blue;

        [Header("Structures")]
        [SerializeField] private List<PileView> tableauPiles;
        [SerializeField] private PileView stockPile; 

        [HideInInspector][Header("Variables")]
        private List<CardPresenter> _cardPresenters = new List<CardPresenter>();
        private List<CardView> _cardViews = new List<CardView>();

        void Start()
        {
            InitializeGame();
        }

        public void InitializeGame()
        {
            // criação das cartas
            List<CardModel> deckModels = DeckGenerator.CreateFullDeck();
            
            foreach (var model in deckModels)
            {
                GameObject cardObj = Instantiate(cardPrefab, transform.position, Quaternion.identity);

                CardView view = cardObj.GetComponent<CardView>();
                _cardViews.Add(view);
                
                Sprite cardSprite = GetSpriteFrontForCard(model.Rank, model.Suit);
                Sprite cardBackSprite = GetSpriteBackForCard(cardsBack);
                view.Setup(cardSprite, cardBackSprite);
            
                CardPresenter presenter = new CardPresenter(model, view);
                _cardPresenters.Add(presenter);
            }

            // organizar cartas em pilhas 

            Dealer dealer = new Dealer();
            dealer.Deal(_cardViews, tableauPiles, stockPile);

            FlipInitialCards();
        }

        private void FlipInitialCards()
        {
            // vira a ultima carta de cada pilha do tableau
            foreach(var pile in tableauPiles)
            {
                if(pile.GetPileCount() > 0)
                {
                    // pega o ultimo card e chama o flip nele 
                    pile.GetLastCard().RequestFlip();
                }
            }
        }

        private Sprite GetSpriteFrontForCard(Rank rank, Suit suit)
        {
            return deckData.cards.Find(c => c.rank == rank && c.suit == suit).cardSprite;
        }

        private Sprite GetSpriteBackForCard(Back color)
        {
            return deckData.cardsBack.Find(c => c.color == color).cardBackSprite;
        }
    }
}