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

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnDisable()
    {
        MoveExecutor.OnBoardStateChanged -= EvaluateWinCondition;
        CommandManager.OnCommandExecuted -= HandleMoveAdded;
        CommandManager.OnCommandUndone -= HandleMoveUndone;  
        BoardManager.Instance.OnAnimationsCompleted -= HandleAutoCompleteFinished;  
    }
    
    void Start()
    {
        MoveExecutor.OnBoardStateChanged += EvaluateWinCondition;
        CommandManager.OnCommandExecuted += HandleMoveAdded;
        CommandManager.OnCommandUndone += HandleMoveUndone;      
        BoardManager.Instance.OnAnimationsCompleted += HandleAutoCompleteFinished;

        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState)
    {
        if(CurrentState == newState) return;

        // Limpeza dos estados anteriores --------------------------------
        if (CurrentState == GameState.Playing)
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
        } 
        else if (CurrentState == GameState.GameOver || CurrentState == GameState.AutoComplete)
        {
            BoardManager.Instance.ClearBoard();
        }
        // ---------------------------------------------------------------

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);

        switch(CurrentState)
        {
            case GameState.Menu:
                CommandManager.ClearHistory();
                break;
            case GameState.Dealing:
                InitializeGame();
                break;
            case GameState.Playing:
                _timerCoroutine = StartCoroutine(TimerRoutine());
                break;
            case GameState.AutoComplete:
                CommandManager.ClearHistory();
                BoardManager.Instance.AutoComplete();
                break;
            case GameState.GameOver:
                ResetStats();
                break;
        }
    }

    public void StartNewGame(int selectedDifficulty, int selectedDeck, int selectedSprite)
    {
        Difficulty = selectedDifficulty; 

        _deck = _deckList[selectedDeck];

        _spriteBack = _deck.cardsBack[selectedSprite].color;

        ChangeState(GameState.Dealing);
    }
    private void InitializeGame()
    {
        CommandManager.ClearHistory();

        // criação das cartas
        List<CardModel> deckModels = DeckGenerator.CreateFullDeck();
        
        List<CardView> cardViews = DeckFactory.Instance.CreateDeck(deckModels, _deck, _spriteBack);

        // organizar cartas em pilhas 
        Dealer dealer = new Dealer();
        dealer.Deal(cardViews, BoardManager.Instance._tableauPiles, BoardManager.Instance._stockPile, () => ChangeState(GameState.Playing));
    } 

    private void EvaluateWinCondition()
    {
        if(CurrentState != GameState.Playing) return;

        if(WinValidator.CheckForVictory(BoardManager.Instance._foundationPiles))
        {
            ChangeState(GameState.GameOver);
            return;
        }

        bool canAuto = BoardManager.Instance.CanTriggerAutoComplete();
        OnAutoCompleteAvailable?.Invoke(canAuto);
    }      

    private void HandleAutoCompleteFinished()
    {
        ChangeState(GameState.Playing); 

        if (BoardManager.Instance.IsGameWon())
        {
            ChangeState(GameState.GameOver);
        }
    }

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

    private IEnumerator TimerRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            ElapsedSeconds++;
            OnTimeChanged?.Invoke(ElapsedSeconds);
        }
    }

    private void HandleMoveAdded() => OnMovesChanged?.Invoke(++Moves);
    private void HandleMoveUndone() => OnMovesChanged?.Invoke(--Moves);


}

