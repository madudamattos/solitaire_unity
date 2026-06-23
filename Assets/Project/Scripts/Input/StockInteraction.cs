using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Solitaire.Views;
using Solitaire.Managers;
using Solitaire.Logic;
using System.Collections.Generic;
using Solitaire.Core;

namespace Solitaire.Input
{   
    /// <summary>
    /// Handles pointer click interactions on the Stock Pile. 
    /// Determines the pile's behavior upon interaction: it either draws a specific number of cards 
    /// (based on the game difficulty) and moves them to the Waste pile, or recycles all cards 
    /// from the Waste pile back into the Stock if the Stock is empty.
    /// </summary>
    [RequireComponent(typeof(PileView))]
    public class StockInteraction : MonoBehaviour, IPointerClickHandler
    {
        private PileView _stockPile;

        [Header("References")]
        [SerializeField] private PileView _wastePile;

        private void Awake()
        {
            _stockPile = GetComponent<PileView>();
        }

        /// <summary>
        /// Triggers when the user clicks on the Stock Pile's collider.
        /// Evaluates the current card counts to determine whether to draw new cards or recycle the waste.
        /// </summary>
        /// <param name="eventData">Pointer event payload provided by Unity's Event System.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_stockPile.GetPileCount() > 0)
            {
                DrawCards(GameManager.Instance.Difficulty);
            }
            else if (_wastePile.GetPileCount() > 0)
            {
                RecycleWaste();
            }    
        }

        private void DrawCards(int maxAmount)
        {
            List<CardView> draggedCards = new List<CardView>();
            
            // Prevents out-of-bounds errors if the stock has fewer cards than the draw amount
            int amountToDraw = Mathf.Min(maxAmount, _stockPile.GetPileCount());

            List<CardView> stockCards = _stockPile.GetCardsInPile();

            for (int i = 0; i < amountToDraw; i++)
            {
                CardView card = stockCards[stockCards.Count - 1 - i];
                draggedCards.Add(card);
            }

            ICommand moveCommand = new MoveCardsCommand(draggedCards, _stockPile, _wastePile);
            CommandManager.AddCommand(moveCommand);

            draggedCards.Clear();
        }    

        private void RecycleWaste()
        {
            List<CardView> wasteCards = _wastePile.GetCardsInPile();

            ICommand moveCommand = new MoveCardsCommand(wasteCards, _wastePile, _stockPile);
            CommandManager.AddCommand(moveCommand);
        }
    }
}