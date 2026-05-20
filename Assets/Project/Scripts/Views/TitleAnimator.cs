using UnityEngine;
using TMPro;
using DG.Tweening;

public class MainMenuAnimator : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Transform[] _cards;

    [Header("Configurações de Animação")]
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

        // ANIMAÇÃO E FLIP DAS CARTAS
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

            // 1. Subida e Descida mantendo o Yoyo
            _mainSequence.Insert(cardStartTime, card.DOLocalMoveY(originalYPositions[i] + _jumpHeightCard, _jumpDurationCard)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo));

            // 2. Flip inicia exatamente após o Yoyo terminar (2 * _jumpDurationCard)
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

            // Rastreia o momento mais tardio que a sequência alcança
            float thisCardEndTime = flipStartTime + _flipDuration;
            if (thisCardEndTime > maxTime)
            {
                maxTime = thisCardEndTime;
            }

            // O avanço para a próxima carta continua o mesmo (cascata)
            currentTime += _jumpDurationCard;
        }

        // CONTROLE DO LOOP E PAUSA FINAL
        
        // Insere um espaço vazio no final da timeline inteira para aplicar a pausa Cartas -> Letras
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