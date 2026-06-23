using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Solitaire.Models;
using Solitaire.Logic;
using Solitaire.Core;
using Solitaire.Views;
using Solitaire.Managers;

namespace Solitaire.Input
{
    /// <summary>
    /// Handles physical mouse/touch input events for drag-and-drop mechanics.
    /// Interacts directly with the <c>MoveValidator</c> to authorize actions and dispatches 
    /// valid interactions to the <c>CommandManager</c>.
    /// </summary>
    /// </remarks> This script was designed to be attached to the card object in unity, interacting direcly with its components: colliders, raycasters, transform, etc.  
    public class CardInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Visual References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider; 
        [SerializeField] private CardView _cardView;

        [Header("Physics")]
        [SerializeField] private LayerMask _dropLayerMask;
        private Camera _mainCamera;
        
        [Header("Variables")]
        
        /// <summary> The physical pile that currently holds this card. </summary>
        public PileView CurrentPile { get; set; }
        
        private List<CardView> _draggedCards = new List<CardView>();
        private List<Vector3> _offsets = new List<Vector3>();
        private List<Vector3> _originalPositions = new List<Vector3>();
        private List<int> _originalSortingOrders = new List<int>();

        // State control flags
        private bool _isDragging = false;
        private int _currentPointerId;

        private void Awake()
        {
            _mainCamera = Camera.main;

            if (_spriteRenderer == null)
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (_collider == null)
                _collider = GetComponentInChildren<Collider2D>();
        }

        /// <summary>
        /// Triggered when the user begins to drag the card. 
        /// Validates the interaction and prepares the card (or stack of cards) for physical movement.
        /// </summary>
        /// <param name="eventData">Pointer event payload from Unity's Event System.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragging) return;
            if (!MoveValidator.CanDrag(_cardView, CurrentPile)) return;
            if (CommandManager.IsProcessing) return;
            
            _isDragging = true;
            _currentPointerId = eventData.pointerId;

            // Determines if we are dragging a single card or a cascading stack (Tableau)
            if (CurrentPile.Type == PileType.Tableau)
            {
                _draggedCards = CurrentPile.GetCardsFrom(_cardView);
            }
            else
            {
                // For Foundation and Waste, only the very top card can be dragged
                if (CurrentPile.Type == PileType.Foundation && _cardView != CurrentPile.GetLastCard()) return;

                _draggedCards = new List<CardView>();
                _draggedCards.Add(_cardView);
            }

            _offsets.Clear();
            _originalPositions.Clear();
            _originalSortingOrders.Clear();

            Vector3 mouseWorldPos = GetMouseWorldPosition(eventData.position);
        
            // Caches original states and elevates the dragged cards' sorting order above the rest of the board
            for (int i = 0; i < _draggedCards.Count; i++)
            {
                CardView card = _draggedCards[i];
                Transform cardTransform = card.transform;
                SpriteRenderer cardRenderer = card.GetComponentInChildren<SpriteRenderer>();

                _originalPositions.Add(cardTransform.position);
                _offsets.Add(cardTransform.position - mouseWorldPos);
                _originalSortingOrders.Add(cardRenderer.sortingOrder);
                cardRenderer.sortingOrder = 100 + i;
            }
        }

        /// <summary>
        /// Continually called while the drag is occurring. Updates the physical position 
        /// of all cards in the dragged stack based on the cursor's location.
        /// </summary>
        /// <param name="eventData">Pointer event payload from Unity's Event System.</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || eventData.pointerId != _currentPointerId) return;
            
            Vector3 mouseWorldPos = GetMouseWorldPosition(eventData.position);

            // Applies individual positional offsets to maintain stack formatting during drag
            for (int i = 0; i < _draggedCards.Count; i++)
            {
                _draggedCards[i].transform.position = mouseWorldPos + _offsets[i];
            }
        }

        /// <summary>
        /// Triggered when the user releases the drag. 
        /// Performs a raycast to find a valid target pile and consults the <c>MoveValidator</c>.
        /// If valid, executes the command; otherwise, reverts the cards to their original positions.
        /// </summary>
        /// <param name="eventData">Pointer event payload from Unity's Event System.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging || eventData.pointerId != _currentPointerId) return;

            _isDragging = false;

            // Casts a point to detect physical colliders at the drop location
            Vector3 mousePos = GetMouseWorldPosition(eventData.position);
            Collider2D hit = Physics2D.OverlapPoint(mousePos, _dropLayerMask);

            // Checks if we hit a valid PileView component
            if (hit != null && hit.TryGetComponent(out PileView targetPile))
            {   
                CardModel modelToMove = _cardView.Presenter.Model; 

                if (MoveValidator.IsValidMove(modelToMove, targetPile, _draggedCards.Count))
                {
                    ICommand moveCommand = new MoveCardsCommand(_draggedCards, CurrentPile, targetPile);
                    CommandManager.AddCommand(moveCommand);
                }
                else
                {
                    ReturnCardsToOriginalPositions();
                }
            }
            else
            {
                ReturnCardsToOriginalPositions();
            }

            _draggedCards.Clear();
        }
        
        /// <summary>
        /// Reverts an invalid drag attempt by snapping the cards back to their cached 
        /// starting positions and resetting their visual sorting order.
        /// </summary>
        private void ReturnCardsToOriginalPositions()
        {
            for (int i = 0; i < _draggedCards.Count; i++)
            {
                _draggedCards[i].transform.position = _originalPositions[i];
                _draggedCards[i].GetComponentInChildren<SpriteRenderer>().sortingOrder = _originalSortingOrders[i]; 
            }
        }

        /// <summary>
        /// Converts the 2D screen-space mouse coordinate into a 3D world-space coordinate.
        /// </summary>
        private Vector3 GetMouseWorldPosition(Vector2 screenPosition)
        {
            Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
            return worldPosition;
        }
    }
}