using UnityEngine;
using DG.Tweening; // Usando DOTween para o movimento 
using Solitaire.Models;

namespace Solitaire.Views 
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite cardFront;
        [SerializeField] private Sprite cardBack;

        // A view guarda uma referencia para o model
        private CardModel _model;

        public void Setup(CardModel model, Sprite front, Sprite back)
        {
            _model = model; 
            cardFront = front; 
            cardBack = back;
        }

        public void SetFaceUp(bool isFaceUp)
        {
            spriteRenderer.sprite = isFaceUp ? cardFront : cardBack;
            //transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }
    }
}
