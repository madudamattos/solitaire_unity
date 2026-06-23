using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Manages the idle animation sequence for the main menu.
/// Combines a staggered text wave effect with a cascading jump-and-flip physical animation 
/// for background card elements.
/// </summary>
public class MainMenuAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Transform[] _cards;

    [Header("Animation Settings")]
    [SerializeField] private float _jumpHeight = 12f;
    [SerializeField] private float _jumpHeightCard = 80f;
    [SerializeField] private float _jumpDuration = 0.2f;
    [SerializeField] private float _jumpDurationCard = 0.3f;
    [SerializeField] private float _flipDuration = 0.1f; 

    [SerializeField] private float _pauseBetweenSequences = 0.35f;

    private Sequence _mainSequence;

    private void Start()
    {
        BuildAndPlaySequence();
    }

    /// <summary>
    /// Builds and executes the complex DOTween sequence. Synchronizes the TextMeshPro character 
    /// offsets with the spatial transformation and scale manipulation (fake 3D flip) of the card objects.
    /// </summary>
    private void BuildAndPlaySequence()
    {
        _titleText.ForceMeshUpdate();
        DOTweenTMPAnimator textAnimator = new DOTweenTMPAnimator(_titleText);

        _mainSequence = DOTween.Sequence();
        
        float currentTime = 0f;
        float maxTime = 0f; // Variable to track the exact end of the timeline

        // LETTER ANIMATION
        for (int i = 0; i < textAnimator.textInfo.characterCount; i++)
        {
            if (!textAnimator.textInfo.characterInfo[i].isVisible) continue;

            Tween charJump = textAnimator.DOOffsetChar(i, new Vector3(0, _jumpHeight, 0), _jumpDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo);

            _mainSequence.Insert(currentTime, charJump);
            
            // The maximum time for the letter is the current time + round trip
            maxTime = currentTime + (_jumpDuration * 2);
            currentTime += _jumpDuration;
        }

        // The pause before the cards start uses the maxTime
        currentTime = maxTime + _pauseBetweenSequences;

        // CARD ANIMATION AND FLIP
        float[] originalYPositions = new float[_cards.Length];
        float[] originalXScales = new float[_cards.Length];
        
        for (int i = 0; i < _cards.Length; i++)
        {
            originalYPositions[i] = _cards[i].localPosition.y;
            originalXScales[i] = _cards[i].localScale.x;
        }

        float halfFlip = _flipDuration / 2f;

        for (int i = _cards.Length - 1; i >= 0; i--)
        {
            Transform card = _cards[i];
            float cardStartTime = currentTime;

            // 1. Up and Down movement maintaining the Yoyo effect
            _mainSequence.Insert(cardStartTime, card.DOLocalMoveY(originalYPositions[i] + _jumpHeightCard, _jumpDurationCard)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo));

            // 2. Flip begins exactly after the Yoyo finishes (2 * _jumpDurationCard)
            float flipStartTime = cardStartTime + (_jumpDurationCard * 2);

            _mainSequence.Insert(flipStartTime, card.DOScaleX(0f, halfFlip).SetEase(Ease.InSine));
            
            _mainSequence.InsertCallback(flipStartTime + halfFlip, () =>
            {
                GameObject child0 = card.GetChild(0).gameObject;
                GameObject child1 = card.GetChild(1).gameObject;
                bool isChild0Active = child0.activeSelf;
                child0.SetActive(!isChild0Active);
                child1.SetActive(isChild0Active);
            });

            _mainSequence.Insert(flipStartTime + halfFlip, card.DOScaleX(originalXScales[i], halfFlip).SetEase(Ease.OutSine));

            // Tracks the latest moment reached by the sequence
            float thisCardEndTime = flipStartTime + _flipDuration;
            if (thisCardEndTime > maxTime)
            {
                maxTime = thisCardEndTime;
            }

            // The advance to the next card maintains the cascade effect
            currentTime += _jumpDurationCard;
        }

        // LOOP CONTROL AND FINAL PAUSE
        
        // Inserts an empty space at the end of the entire timeline to apply the Cards -> Letters pause
        _mainSequence.Insert(maxTime, DOVirtual.DelayedCall(_pauseBetweenSequences, () => {}));
        
        _mainSequence.SetLoops(-1);
    }

    /// <summary>
    /// Safely kills the active DOTween sequence upon object destruction to prevent memory leaks 
    /// and null reference exceptions.
    /// </summary>
    private void OnDestroy()
    {
        if (_mainSequence != null)
        {
            _mainSequence.Kill();
        }
    }
}