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

/// <summary>
/// Core controller class responsible for managing the game's Finite State Machine (FSM), 
/// global statistics, and high-level progression flow.
/// Follows the Singleton pattern to ensure a single point of truth for the game state.
/// </summary>
/// <remarks>
/// Deck creation is initiated by a user action routed through the UI. It receives the logical deck model, 
/// the selected deck configuration (the <c>DeckData</c> mapping logical cards to sprites), and the 
/// enumerated value for the card back color directly from the UI context. 
/// </remarks>
public class GameManager : MonoBehaviour
{
    [HideInInspector][Header("Game State Control")]
    
    public static GameManager Instance { get; private set; }
    
    public GameState CurrentState { get; private set; }
    
    public event Action<GameState> OnStateChanged;
    
    public event Action<bool> OnAutoCompleteAvailable;

    [Header("Game Stats Control")]
    
    /// <summary> The total number of valid moves executed by the player in the current session. </summary>
    public int Moves { get; private set; }
    
    /// <summary> The total elapsed time in seconds since the current game transitioned to the Playing state. </summary>
    public int ElapsedSeconds { get; private set; }
    
    private Coroutine _timerCoroutine;
    
    public event Action<int> OnMovesChanged;
    
    public event Action<int> OnTimeChanged;

    [Header("Game Settings")]
    
    /// <summary> The selected ruleset difficulty (e.g., Draw 1 or Draw 3). </summary>
    public int Difficulty { get; private set; }
    
    private DeckData _deck;
    private Back _spriteBack;
    
    [SerializeField] List<DeckData> _deckList = new List<DeckData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        MoveExecutor.OnBoardStateChanged += EvaluateWinCondition;
        CommandManager.OnCommandExecuted += HandleMoveAdded;
        CommandManager.OnCommandUndone += HandleMoveUndone;      
    }

    private void OnDisable()
    {
        MoveExecutor.OnBoardStateChanged -= EvaluateWinCondition;
        CommandManager.OnCommandExecuted -= HandleMoveAdded;
        CommandManager.OnCommandUndone -= HandleMoveUndone;  
        
        if (BoardManager.Instance != null)
            BoardManager.Instance.OnAnimationsCompleted -= HandleAutoCompleteFinished;  
    }
    
    private void Start()
    {
        if (BoardManager.Instance != null)
            BoardManager.Instance.OnAnimationsCompleted += HandleAutoCompleteFinished;

        ChangeState(GameState.Menu);
    }

    /// <summary>
    /// Executes the transition logic between game states. Handles setup and teardown routines 
    /// for specific states, such as halting the timer or clearing the board.
    /// </summary>
    /// <param name="newState">The target state to transition into.</param>
    public void ChangeState(GameState newState)
    {
        // Teardown of previous states
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

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);

        // Setup for new states
        switch (CurrentState)
        {
            case GameState.Menu:
                CommandManager.ClearHistory();
                ResetStats();
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

    /// <summary>
    /// Bootstraps a new game session with user-defined settings and triggers the dealing phase.
    /// </summary>
    /// <param name="selectedDifficulty">The number of cards to draw per stock click.</param>
    /// <param name="selectedDeck">The index of the visual deck mapping to use.</param>
    /// <param name="selectedSprite">The index defining the card back visual variant.</param>
    public void StartNewGame(int selectedDifficulty, int selectedDeck, int selectedSprite)
    {
        Difficulty = selectedDifficulty; 
        _deck = _deckList[selectedDeck];
        _spriteBack = _deck.cardsBack[selectedSprite].color;

        ChangeState(GameState.Dealing);
    }

    /// <summary>
    /// Handles the logical generation and physical instantiation of the deck, 
    /// followed by delegating the initial distribution of cards to the Dealer logic.
    /// </summary>
    private void InitializeGame()
    {
        CommandManager.ClearHistory();

        List<CardModel> deckModels = DeckGenerator.CreateFullDeck();
        List<CardView> cardViews = DeckFactory.Instance.CreateDeck(deckModels, _deck, _spriteBack);

        Dealer dealer = new Dealer();
        dealer.Deal(cardViews, BoardManager.Instance._tableauPiles, BoardManager.Instance._stockPile, () => ChangeState(GameState.Playing));
    } 

    /// <summary>
    /// Cross-references the current board state with the victory and auto-complete logic rules.
    /// Triggers state changes or events based on the validation results.
    /// </summary>
    private void EvaluateWinCondition()
    {
        if (CurrentState != GameState.Playing) return;

        if (WinValidator.CheckForVictory(BoardManager.Instance._foundationPiles))
        {
            ChangeState(GameState.GameOver);
            return;
        }

        bool canAuto = BoardManager.Instance.CanTriggerAutoComplete();
        OnAutoCompleteAvailable?.Invoke(canAuto);
    }      

    /// <summary>
    /// Callback method invoked when the BoardManager finishes its automated sequence.
    /// Verifies the final board state to declare GameOver.
    /// </summary>
    private void HandleAutoCompleteFinished()
    {
        if (BoardManager.Instance.IsGameWon())
        {
            ChangeState(GameState.GameOver);
        }
    }

    /// <summary>
    /// Resets global tracking statistics (moves and time) and fires corresponding update events.
    /// </summary>
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

    /// <summary>
    /// Coroutine responsible for ticking the game timer every second.
    /// </summary>
    private IEnumerator TimerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            ElapsedSeconds++;
            OnTimeChanged?.Invoke(ElapsedSeconds);
        }
    }

    private void HandleMoveAdded() => OnMovesChanged?.Invoke(++Moves);
    
    private void HandleMoveUndone() => OnMovesChanged?.Invoke(--Moves);
}