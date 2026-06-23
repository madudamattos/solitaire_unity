using UnityEngine;
using System.Collections.Generic;
using Solitaire.Models;
using Solitaire.Presenters;
using Solitaire.Core;
using Solitaire.Views;

namespace Solitaire.Factories
{
    /// <summary>
    /// Factory class responsible for instantiating the visual card objects within the Unity scene. 
    /// Follows the Singleton pattern to ensure a single point of creation for the deck.
    /// </summary>
    /// <remarks>
    /// Deck creation is initiated by a user action routed through the UI. It receives the logical deck model, 
    /// the selected deck configuration (the <c>DeckData</c> mapping logical cards to sprites), and the 
    /// enumerated value for the card back color directly from the <c>GameManager</c>. 
    /// This class also requires a predefined Card Prefab to instantiate the physical card objects within the Unity scene.
    /// </remarks>
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

        /// <summary>
        /// Instantiates the visual card GameObjects based on the provided logical models, 
        /// assigns the correct sprites, and binds them using the Presenter.
        /// </summary>
        /// <param name="models">The list of logical card models to be instantiated.</param>
        /// <param name="deckData">The ScriptableObject containing the visual asset mappings for the deck.</param>
        /// <param name="backSpriteColor">The chosen color for the card back sprite.</param>
        /// <returns>A list containing the instantiated and initialized <c>CardView</c> components.</returns>
        public List<CardView> CreateDeck(List<CardModel> models, DeckData deckData, Back backSpriteColor)
        {
            _cardsBack = backSpriteColor;
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