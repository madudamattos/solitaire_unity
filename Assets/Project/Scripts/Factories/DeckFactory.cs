using UnityEngine;
using System.Collections.Generic;
using Solitaire.Models;
using Solitaire.Presenters;
using Solitaire.Core;
using Solitaire.Views;

namespace Solitaire.Factories
{
    public class DeckFactory : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private GameObject _cardPrefab;
        private DeckData _deckData;
        private Back _cardsBack;

        public static DeckFactory Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public List<CardView> CreateDeck(List<CardModel> models, DeckData deckData, Back backSprite)
        {
            _cardsBack = backSprite;
            _deckData = deckData;  
             
            List<CardView> cardViews = new List<CardView>();

            foreach (var model in models)
            {
                GameObject cardObj = Instantiate(_cardPrefab, transform.position, Quaternion.identity);
                CardView view = cardObj.GetComponent<CardView>();

                Sprite cardFrontSprite = GetSpriteFrontForCard(model.Rank, model.Suit);
                Sprite cardBackSprite = GetSpriteBackForCard(_cardsBack);

                view.Setup(cardFrontSprite, cardBackSprite);
            
                CardPresenter presenter = new CardPresenter(model, view);
                
                cardViews.Add(view);
            }

            return cardViews;
        }

        private Sprite GetSpriteFrontForCard(Rank rank, Suit suit)
        {
            return _deckData.cards.Find(c => c.rank == rank && c.suit == suit).cardSprite;
        }

        private Sprite GetSpriteBackForCard(Back color)
        {
            return _deckData.cardsBack.Find(c => c.color == color).cardBackSprite;
        }
    }
}