using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Solitaire.Core;

namespace Solitaire.Views
{
    /// <summary>
    /// Visual component representing a physical container for a stack of cards on the board.
    /// Manages the physical positioning, hierarchy, and visual layout of the cards it contains.
    /// </summary>
    public class PileView : MonoBehaviour
    {
        /// <summary>
        /// Defines the role of this pile (e.g., Tableau, Foundation, Stock, Waste).
        /// </summary>
        [SerializeField] public PileType Type;

        /// <summary>
        /// The specific suit assigned to this pile. Used only for Foundation piles 
        /// to restrict which cards can be placed here.
        /// </summary>
        [SerializeField] public Suit Suit;

        // Visual list of cards that are currently parented to this pile.
        private List<CardView> CardsInPile = new List<CardView>();

        /// <summary>
        /// Calculates the exact world-space coordinate where the next card should be placed,
        /// based on the pile's type and its current visual layout rules.
        /// </summary>
        /// <returns>The calculated <c>Vector3</c> target position.</returns>
        public Vector3 GetNextCardPosition()
        {
            if (Type == PileType.Foundation || Type == PileType.Stock || CardsInPile.Count <= 0) 
                return transform.position;

            float offset = 0.92f;
            float yOffset = 0;
            float zOffset = 0;

            // The waste pile shifts cards slightly on the Y axis to fan them out
            if (Type == PileType.Waste)
            {
                yOffset = -offset;
                return CardsInPile.Last().transform.position + new Vector3(0, yOffset, zOffset); 
            }

            // Tableau piles cascade cards downwards. Face-up cards use a larger offset than face-down cards.
            for (int i = 0; i < CardsInPile.Count; i++)
            {
                yOffset -= CardsInPile[i].Presenter.Model.IsFaceUp ? offset : offset / 2;
            }

            return transform.position + new Vector3(0, yOffset, zOffset); 
        }

        /// <summary> Adds a card view to the internal tracking list. </summary>
        public void AddCard(CardView card)
        {
            CardsInPile.Add(card);
        }

        /// <summary> Retrieves the card currently sitting at the very top of the pile. </summary>
        public CardView GetLastCard()
        {
            return CardsInPile.Last();
        }

        /// <summary> Returns the total number of cards currently in this pile. </summary>
        public int GetPileCount()
        {
            return CardsInPile.Count;
        }

        /// <summary> Returns the full internal list of cards. </summary>
        public List<CardView> GetCardsInPile()
        {
            return CardsInPile;
        }

        /// <summary> Removes a specific card view from the internal tracking list. </summary>
        public void RemoveCard(CardView card)
        {
            if (CardsInPile.Contains(card))
                CardsInPile.Remove(card);
        }

        /// <summary>
        /// Retrieves a sub-list containing the target card and all cards stacked on top of it.
        /// Used for multi-card drag-and-drop operations in the Tableau.
        /// </summary>
        /// <param name="startCard">The base card from which the slice should begin.</param>
        /// <returns>A list of <c>CardView</c> objects representing the movable stack.</returns>
        public List<CardView> GetCardsFrom(CardView startCard)
        {
            int startIndex = CardsInPile.IndexOf(startCard); 

            // Returns an empty list if the card is not found (safety fallback)
            if (startIndex == -1) return new List<CardView>();
            
            return CardsInPile.GetRange(startIndex, CardsInPile.Count - startIndex);
        }

        /// <summary>
        /// Executes the specific visual arrangement for the Waste pile, ensuring only 
        /// the top three cards are fanned out and visible.
        /// </summary>
        public void UpdateWasteVisuals()
        {
            if (Type != PileType.Waste) return;

            int count = CardsInPile.Count;
            if (count == 0) return;

            float yOffset = 0.92f;
            float zOffset = 0.01f;

            for (int i = count - 1; i >= 0; i--) 
            {
                CardView card = CardsInPile[i];
                Vector3 targetPos = transform.position;

                if (i >= count - 3) 
                {
                    // Inverts the index for the visual slot (0, 1, 2)
                    int slotPos = i - Mathf.Max(0, count - 3); 
                    targetPos.y -= slotPos * yOffset;
                    targetPos.z -= slotPos * zOffset;
                }

                card.MoveTo(targetPos, 0.2f, 0f);
                card.SetSortingOrder(i);
            }
        }
    }
}