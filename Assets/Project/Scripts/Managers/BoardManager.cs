using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Solitaire.Views;
using Solitaire.Logic;
using System;

/// <summary>
/// Manager representing the physical layout of the game. 
/// Encapsulates references to all pile structures and executes the automated resolution sequence.
/// </summary>
public class BoardManager : MonoBehaviour
{
    [Header("Structures")]
    public PileView _stockPile;
    public PileView _wastePile;
    public List<PileView> _foundationPiles;
    public List<PileView> _tableauPiles;

    public event Action OnAnimationsCompleted;

    /// <summary> Global Singleton access point for the physical board facade. </summary>
    public static BoardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Initiates the coroutine responsible for automatically moving remaining cards to the foundations.
    /// </summary>
    public void AutoComplete()
    {
        StartCoroutine(AutoCompleteRoutine());
    }

    /// <summary>
    /// Sequentially scans the Tableau and Waste piles for valid Foundation moves.
    /// Uses yields to create a staggered animation sequence.
    /// </summary>
    private IEnumerator AutoCompleteRoutine()
    {
        bool isWon = false;

        while (!isWon)
        {
            List<PileView> pilesToScan = new List<PileView>(_tableauPiles);
            List<CardView> wasteCards = new List<CardView>(_wastePile.GetCardsInPile());
            // List<CardView> stockCards = new List<CardView>(_stockPile.GetCardsInPile());

            bool movedCardInThisLoop = false;


            foreach (PileView pile in pilesToScan)
            {
                if (pile.GetPileCount() == 0) continue;

                CardView topCard = pile.GetLastCard();

                PileView targetFoundation = MoveValidator.GetValidFoundation(topCard.Presenter.Model, _foundationPiles);

                if (targetFoundation != null)
                {                   
                    MoveExecutor.ExecuteMove(new List<CardView> { topCard }, pile, targetFoundation);
                    movedCardInThisLoop = true;
                    
                    yield return new WaitForSeconds(0.3f); 
                    break;
                }
            }

            foreach (CardView card in wasteCards)
            {
                PileView targetFoundation = MoveValidator.GetValidFoundation(card.Presenter.Model, _foundationPiles);

                if (targetFoundation != null)
                {                 
                    MoveExecutor.ExecuteMove(new List<CardView> { card }, _wastePile, targetFoundation);
                    movedCardInThisLoop = true;
                    yield return new WaitForSeconds(0.3f); 
                    break;
                }
            }

            // foreach (CardView card in stockCards)
            // {
            //     PileView targetFoundation = MoveValidator.GetValidFoundation(card.Presenter.Model, _foundationPiles);

            //     if (targetFoundation != null)
            //     {                 
            //         MoveExecutor.ExecuteMove(new List<CardView> { card }, _stockPile, targetFoundation);
            //         movedCardInThisLoop = true;
            //         yield return new WaitForSeconds(0.3f); 
            //         break;
            //     }
            // }

            // If a full scan yields no moves, the routine is complete
            if (!movedCardInThisLoop) break;
        }
        
        OnAnimationsCompleted?.Invoke();
    }

    /// <summary>
    /// Forces the destruction of all physical card GameObjects currently anchored to the board's piles.
    /// Used during state teardown routines.
    /// </summary>
    public void ClearBoard()
    {
        ClearPile(_stockPile);
        ClearPile(_wastePile);

        foreach (PileView pile in _foundationPiles)
        {
            ClearPile(pile);
        }

        foreach (PileView pile in _tableauPiles)
        {
            ClearPile(pile);
        }
    }  


    /// <summary>
    /// Safely clears a specific pile by capturing its elements into a temporary list before destruction,
    /// avoiding modification-during-iteration errors.
    /// </summary>
    /// <param name="pile">The target pile to be cleared.</param>
    private void ClearPile(PileView pile)
    {
        if (pile == null) return;

        // Instancia uma nova lista contendo os mesmos elementos para permitir a iteração segura
        List<CardView> cardsToDestroy = new List<CardView>(pile.GetCardsInPile());

        foreach (CardView card in cardsToDestroy)
        {
            pile.RemoveCard(card);
            
            // Remove o objeto físico da cena 
            if (card != null && card.gameObject != null)
            {
                Destroy(card.gameObject);
            }
        }
    }
    
    /// <summary> Facade method delegating to the <c>WinValidator</c> to check for standard victory. </summary>
    public bool IsGameWon()
    {
        return WinValidator.CheckForVictory(_foundationPiles);
    }

    /// <summary> Facade method delegating to the <c>WinValidator</c> to check auto-complete thresholds. </summary>
    public bool CanTriggerAutoComplete()
    {
        return WinValidator.CanAutoComplete(_tableauPiles, _stockPile, _wastePile);
    }
}