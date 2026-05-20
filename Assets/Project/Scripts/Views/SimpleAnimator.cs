using UnityEngine;
using TMPro;
using DG.Tweening;

public class SimpleAnimator : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TMP_Text _titleText;

    [Header("Configurações de Animação")]
    [SerializeField] private float _jumpHeight = 12f;
    [SerializeField] private float _jumpDuration = 0.2f;
    [SerializeField] private float _pauseBetweenSequences = 0.35f;

    private Sequence _mainSequence;

    private void Start()
    {
        BuildAndPlaySequence();
    }

    private void BuildAndPlaySequence()
    {
        _titleText.ForceMeshUpdate();
        DOTweenTMPAnimator textAnimator = new DOTweenTMPAnimator(_titleText);

        _mainSequence = DOTween.Sequence();
        
        float currentTime = 0f;
        float maxTime = 0f; // Variável para rastrear o fim exato da timeline

        // ANIMAÇÃO DAS LETRAS
        for (int i = 0; i < textAnimator.textInfo.characterCount; i++)
        {
            if (!textAnimator.textInfo.characterInfo[i].isVisible) continue;

            Tween charJump = textAnimator.DOOffsetChar(i, new Vector3(0, _jumpHeight, 0), _jumpDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo);

            _mainSequence.Insert(currentTime, charJump);
            
            // O tempo máximo da letra é o tempo atual + ida e volta
            maxTime = currentTime + (_jumpDuration * 2);
            currentTime += _jumpDuration;
        }

        // A pausa para o início das cartas usa o maxTime
        currentTime = maxTime + _pauseBetweenSequences;

        // CONTROLE DO LOOP E PAUSA FINAL
        
        _mainSequence.Insert(maxTime, DOVirtual.DelayedCall(_pauseBetweenSequences, () => {}));
        
        _mainSequence.SetLoops(-1);
    }

    private void OnDestroy()
    {
        if (_mainSequence != null)
        {
            _mainSequence.Kill();
        }
    }
}