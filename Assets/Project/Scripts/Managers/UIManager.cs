using UnityEngine;
using Solitaire.Core;
using Solitaire.Managers;
using Solitaire.Logic;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager: MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _gameTable;
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private GameObject _victoryPanel;
    
    [Header("Buttons")]
    [SerializeField] private GameObject _autoCompleteButton;
    [SerializeField] private Button _undoButton;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _timerText;

    [Header("Difficulty Toggles")]
    [SerializeField] private Toggle _toggleDiff1;
    [SerializeField] private Toggle _toggleDiff3;

    [Header("Deck Toggles")]
    [SerializeField] private Toggle _toggleDeck1;
    [SerializeField] private Toggle _toggleDeck2;

    [Header("Sprite Toggles")]
    [SerializeField] private Toggle _toggleSprite1;
    [SerializeField] private Toggle _toggleSprite2;
    [SerializeField] private Toggle _toggleSprite3;
    [SerializeField] private Toggle _toggleSprite4;

    [Header("Sprites Layout")]
    [SerializeField] private GameObject _cardSpriteLayout1;
    [SerializeField] private GameObject _cardSpriteLayout2;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += HandleStateChange;
        GameManager.Instance.OnAutoCompleteAvailable += ToggleAutoCompleteButton;
        GameManager.Instance.OnMovesChanged += UpdateMovesText;
        GameManager.Instance.OnTimeChanged += UpdateTimeText;
        CommandManager.OnCommandExecuted += UpdateUndoButton;
        CommandManager.OnCommandUndone += UpdateUndoButton;
        CommandManager.OnHistoryCleared += UpdateUndoButton;

        UpdateUndoButton();
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChange;
            GameManager.Instance.OnAutoCompleteAvailable -= ToggleAutoCompleteButton;
            GameManager.Instance.OnMovesChanged -= UpdateMovesText;
            GameManager.Instance.OnTimeChanged -= UpdateTimeText;
        }    

        CommandManager.OnCommandExecuted -= UpdateUndoButton;
        CommandManager.OnCommandUndone -= UpdateUndoButton;
        CommandManager.OnHistoryCleared -= UpdateUndoButton;
    }

    private void ToggleAutoCompleteButton(bool isAvailable)
    {
        _autoCompleteButton.SetActive(isAvailable);
    }

    private void UpdateUndoButton()
    {
        _undoButton.interactable = CommandManager.HasCommands;
    }

    private void UpdateMovesText(int moves)
    {
        _movesText.text = "MOVES: " + moves.ToString();
    }

    private void UpdateTimeText(int seconds)
    {
        // Formata os segundos totais para o padrão MM:SS
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        _timerText.text = "TIMER: " + time.ToString(@"hh\:mm\:ss");
    }

    public void ToggleAltDeck(bool active)
    {
        _cardSpriteLayout1.SetActive(!active);
        _cardSpriteLayout2.SetActive(active);
    }

    public void ToggleBaseDeck(bool active)
    {
        _cardSpriteLayout1.SetActive(active);
        _cardSpriteLayout2.SetActive(!active);
    }

    public void ToggleSettingsPanel()
    {
        _settingsPanel.SetActive(true); 
    }

    private void HandleStateChange(GameState state)
    {
        _menuPanel.SetActive(state == GameState.Menu);
        _settingsPanel.SetActive(state == GameState.Menu);

        _gameTable.SetActive(state == GameState.Dealing || state == GameState.Playing || state == GameState.AutoComplete);
        _gamePanel.SetActive(state == GameState.Dealing || state == GameState.Playing || state == GameState.AutoComplete);
        
        _victoryPanel.SetActive(state == GameState.GameOver);

        if (state != GameState.Playing) 
            _autoCompleteButton.SetActive(false);
    }

    public void UI_StartNewGame()
    {
        int selectedDifficulty = _toggleDiff1.isOn ? 1 : 3;
        int selectedDeck = _toggleDeck1.isOn ? 0 : 1;
        int selectedSprite = 0;

        Toggle[] spriteToggles = { _toggleSprite1, _toggleSprite2, _toggleSprite3, _toggleSprite4 };
        for (int i = 0; i < spriteToggles.Length; i++)
        {
            if (spriteToggles[i].isOn) selectedSprite = i;
        }

        GameManager.Instance.StartNewGame(selectedDifficulty, selectedDeck, selectedSprite);
    }

    public void UI_ReturnToMenu()
    {
        GameManager.Instance.ChangeState(GameState.Menu);
    }

    public void UI_AutoComplete()
    {
        GameManager.Instance.ChangeState(GameState.AutoComplete);
    }

    public void UI_UndoLastMove()
    {
        CommandManager.UndoLastCommand();
    }

}