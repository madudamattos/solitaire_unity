using Solitaire.Models;
using Solitaire.Views;
using Solitaire.Logic;
using Solitaire.Factories;
using Solitaire.Managers;
using Solitaire.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    [HideInInspector][Header("Game State Control")]
    public static GameManager Instance { get; private set; }
    public GameState CurrentState {get; private set;}
    public event Action<GameState> OnStateChanged;
    public event Action<bool> OnAutoCompleteAvailable;

    [Header("Managers")]
    [SerializeField] private DeckFactory _deckFactory;
    [SerializeField] private BoardManager _boardManager;

    [Header("Game Stats Control")]
    public int Moves {get; private set;}
    public int ElapsedSeconds { get; private set; }
    private Coroutine _timerCoroutine;
    public event Action<int> OnMovesChanged;
    public event Action<int> OnTimeChanged;

    [Header("Game Settings")]

    public int Difficulty { get; private set; }
    private DeckData _deck;
    private Back _spriteBack;
    [SerializeField] List<DeckData> _deckList = new List<DeckData>();

    private void OnEnable()
    {
        MoveExecutor.OnBoardStateChanged += EvaluateWinCondition;
        CommandManager.OnCommandExecuted += HandleMoveAdded;
        CommandManager.OnCommandUndone += HandleMoveUndone;
        CommandManager.OnHistoryCleared += ResetStats;        
        _boardManager.OnAutoCompleteFinished += TriggerVictory;
    }

    private void OnDisable()
    {
        MoveExecutor.OnBoardStateChanged -= EvaluateWinCondition;
        CommandManager.OnCommandExecuted -= HandleMoveAdded;
        CommandManager.OnCommandUndone -= HandleMoveUndone;
        CommandManager.OnHistoryCleared -= ResetStats;  
        _boardManager.OnAutoCompleteFinished -= TriggerVictory;  
    }
    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);

        switch(CurrentState)
        {
            case GameState.Menu:
                // ativa o menu, desativa o jogo
                break;
            case GameState.Dealing:
                InitializeGame();
                break;
            case GameState.Playing:
                SetGamePlaying();
                break;
            // ...
        }
    }

    public void StartNewGame(int selectedDifficulty, int selectedDeck, int selectedSprite)
    {
        Difficulty = selectedDifficulty; 

        _deck = _deckList[selectedDeck];

        _spriteBack = _deck.cardsBack[selectedSprite].color;

        StartDeal();
    }
    private void InitializeGame()
    {
        CommandManager.ClearHistory();

        // criação das cartas
        List<CardModel> deckModels = DeckGenerator.CreateFullDeck();
        
        List<CardView> cardViews = _deckFactory.CreateDeck(deckModels, _deck, _spriteBack);

        // organizar cartas em pilhas 
        Dealer dealer = new Dealer();
        dealer.Deal(cardViews, _boardManager._tableauPiles, _boardManager._stockPile, () => StartPlay());
    } 

    private void SetGamePlaying()
    {
        if (CurrentState == GameState.Playing)
        {
            if (_timerCoroutine == null)
                _timerCoroutine = StartCoroutine(TimerRoutine());
        }
        // Se saiu do estado Playing (pausou, venceu, menu), para o relógio
        else
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        }
    }

    private IEnumerator TimerRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            ElapsedSeconds++;
            OnTimeChanged?.Invoke(ElapsedSeconds);
        }
    }

    private void EvaluateWinCondition()
    {
        if(CurrentState != GameState.Playing || CurrentState == GameState.AutoComplete) return;

        if(WinValidator.CheckForVictory(_boardManager._foundationPiles))
        {
            TriggerVictory();
            return;
        }

        bool canAuto = WinValidator.CanAutoComplete(_boardManager._tableauPiles, _boardManager._stockPile, _boardManager._wastePile);
        OnAutoCompleteAvailable?.Invoke(canAuto);
    }      

    private void TriggerVictory()
    {
        ChangeState(GameState.GameOver);
        Debug.Log("Congratulations! You won.");
    }

    public void StartAutoComplete()
    {
        ChangeState(GameState.AutoComplete);
        CommandManager.ClearHistory();

        _boardManager.RunAutoComplete();
    }

    private void HandleMoveAdded() => OnMovesChanged?.Invoke(++Moves);
    private void HandleMoveUndone() => OnMovesChanged?.Invoke(--Moves);

    private void ResetStats()
    {
        Moves = 0;
        ElapsedSeconds = 0;

        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }

        OnMovesChanged?.Invoke(Moves);
        OnTimeChanged?.Invoke(ElapsedSeconds);
    }

    public void ResetGame()
    {
        _boardManager.ClearBoard();
        CommandManager.ClearHistory();
        ChangeState(GameState.Menu);
    }

    public void StartPlay() => ChangeState(GameState.Playing);
    public void StartDeal() => ChangeState(GameState.Dealing);
    public void ReturnToMenu() => ChangeState(GameState.Menu);

}
