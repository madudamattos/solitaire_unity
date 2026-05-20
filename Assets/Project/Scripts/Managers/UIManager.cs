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

    private void OnEnable()
    {
        GameManager.Instance.OnStateChanged += HandleStateChange;
        GameManager.Instance.OnAutoCompleteAvailable += ToggleAutoCompleteButton;
        GameManager.Instance.OnMovesChanged += UpdateMovesText;
        GameManager.Instance.OnTimeChanged += UpdateTimeText;
        CommandManager.OnCommandExecuted += UpdateUndoButton;
        CommandManager.OnCommandUndone += UpdateUndoButton;
        CommandManager.OnHistoryCleared += UpdateUndoButton;
    }

    private void Start()
    {
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

    private void HandleStateChange(GameState state)
    {
        _menuPanel.SetActive(state == GameState.Menu);
        _gameTable.SetActive(state == GameState.Dealing || state == GameState.Playing);
         _gamePanel.SetActive(state == GameState.Dealing || state == GameState.Playing);
        _victoryPanel.SetActive(state == GameState.GameOver);

        if (state != GameState.Playing)
            _autoCompleteButton.SetActive(false);
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

    public void UI_StartGameWithSettings()
    {
        int selectedDifficulty = _toggleDiff1.isOn ? 1 : 3;
        int selectedDeck = _toggleDeck1.isOn ? 0 : 1;
        int selectedSprite = 0;

        if (_toggleSprite1.isOn) selectedSprite = 0;
        else if (_toggleSprite2.isOn) selectedSprite = 1;
        else if (_toggleSprite3.isOn) selectedSprite = 2;
        else if (_toggleSprite4.isOn) selectedSprite = 3;  

        _menuPanel.SetActive(false);

        GameManager.Instance.StartNewGame(selectedDifficulty, selectedDeck, selectedSprite);
    }

    public void UI_Play()
    {
        _settingsPanel.SetActive(true); 
    }

    public void UI_ReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
        _settingsPanel.SetActive(false);
    }

    public void UI_TriggerAutoComplete()
    {
        GameManager.Instance.StartAutoComplete();
    }

    public void UI_UndoLastMove()
    {
        CommandManager.UndoLastCommand();
    }
    
    public void UI_ResetGame()
    {
        _settingsPanel.SetActive(false);
        GameManager.Instance.ResetGame();
    }

}