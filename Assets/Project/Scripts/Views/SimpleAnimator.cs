using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// A visual utility component responsible for animating TextMeshPro text.
/// Applies a continuous, wave-like vertical jumping sequence to individual characters using DOTween.
/// </summary>
public class SimpleAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _titleText;

    [Header("Animation Settings")]
    [SerializeField] private float _jumpHeight = 12f;
    [SerializeField] private float _jumpDuration = 0.2f;
    [SerializeField] private float _pauseBetweenSequences = 0.35f;

    private Sequence _mainSequence;

    private void Start()
    {
        BuildAndPlaySequence();
    }

    /// <summary>
    /// Constructs the DOTween sequence by iterating through the text mesh characters.
    /// Calculates precise timeline insertions to stagger the animation of each letter.
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

        // The pause before the sequence restarts uses the maxTime
        currentTime = maxTime + _pauseBetweenSequences;

        // LOOP CONTROL AND FINAL PAUSE
        _mainSequence.Insert(maxTime, DOVirtual.DelayedCall(_pauseBetweenSequences, () => {}));
        
        _mainSequence.SetLoops(-1);
    }

    /// <summary>
    /// Safely kills the active DOTween sequence upon object destruction to prevent memory leaks 
    /// and null reference exceptions in the animation loop.
    /// </summary>
    private void OnDestroy()
    {
        if (_mainSequence != null)
        {
            _mainSequence.Kill();
        }
    }
}