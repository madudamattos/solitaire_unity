using UnityEngine;
using DG.Tweening; // Usando DOTween para o movimento 
using Solitaire.Presenters;

namespace Solitaire.Views 
{
    /// <summary>
    /// Card View component (following MVP architecture) responsible for the Unity Engine interactions. 
    /// Encapsulates all necessary functions to alter the visual state of the card and execute in-game animations.
    /// </summary>
    public class CardView : MonoBehaviour
    {
        [Header("Visual References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider;

        [Header("On initialization references")]
        [SerializeField] private Sprite _cardFront;
        [SerializeField] private Sprite _cardBack;
        
        private CardPresenter _presenter;

        /// <summary> 
        /// Access point to the card's Presenter layer. 
        /// </summary>
        public CardPresenter Presenter => _presenter;
        
        /// <summary>
        /// Injects the visual assets (sprites) for the front and back of the card.
        /// </summary>
        /// <param name="front">The sprite representing the card's logical face.</param>
        /// <param name="back">The sprite representing the card's back design.</param>
        public void Setup(Sprite front, Sprite back)
        {
            _cardFront = front; 
            _cardBack = back;
        }
        
        /// <summary> 
        /// Binds the View to its respective Presenter. Since the constructor does not receive the Presenter 
        /// upon instantiation, this method establishes the connection.
        /// </summary>
        /// <param name="presenter">The CardPresenter instance managing this view.</param>
        public void Bind(CardPresenter presenter) 
        { 
            _presenter = presenter; 
        }

        /// <summary>
        /// Updates the rendered sprite based on the card's logical orientation.
        /// </summary>
        /// <param name="isFaceUp">If true, renders the front sprite; otherwise, renders the back sprite.</param>
        public void SetFaceUp(bool isFaceUp){ _spriteRenderer.sprite = isFaceUp ? _cardFront : _cardBack; }

        /// <summary>
        /// Routes the flip request to the Presenter, delegating the logical state change.
        /// </summary>
        public void RequestFlip() { _presenter?.FlipCard(); }

        /// <summary>
        /// Toggles the physics collider, enabling or disabling pointer interactions.
        /// </summary>
        /// <param name="enable">Target state for the collider.</param>
        public void SetCollider(bool enable)
        {
            _collider.enabled = enable;
        }

        /// <summary>
        /// Adjusts the SpriteRenderer's sorting order to ensure correct visual stacking (Z-indexing).
        /// </summary>
        /// <param name="order">The integer value defining the render order.</param>
        public void SetSortingOrder(int order)
        {
            _spriteRenderer.sortingOrder = order;
        }

        /// <summary>
        /// Executes an asynchronous spatial transition using DOTween.
        /// </summary>
        /// <param name="targetPosition">The world-space destination coordinate.</param>
        /// <param name="duration">The total time in seconds the movement should take.</param>
        /// <param name="delay">The wait time in seconds before the movement begins.</param>
        /// <param name="onComplete">Optional callback invoked immediately after the tween finishes.</param>
        public void MoveTo(Vector3 targetPosition, float duration, float delay, System.Action onComplete = null)
        {
            // O método DOMove realiza a interpolação do transform.position
            transform.DOMove(targetPosition, duration)
                     .SetDelay(delay)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() => onComplete?.Invoke());
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();

            transform.DOKill();
        }

    }
}
