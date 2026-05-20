using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Solitaire.Views;
using Solitaire.Logic;
using System;

public class BoardManager : MonoBehaviour
{
    [Header("Structures")]
    public PileView _stockPile;
    public PileView _wastePile;
    public List<PileView> _foundationPiles;
    public List<PileView> _tableauPiles;

    public event Action OnAutoCompleteFinished;

    // public static BoardManager Instance { get; private set; }

    // private void Awake()
    // {
    //     if (Instance == null) Instance = this;
    //     else Destroy(gameObject);
    // }

    public void RunAutoComplete()
    {
        StartCoroutine(AutoCompleteRoutine());
    }

    private IEnumerator AutoCompleteRoutine()
    {
        bool isWon = false;
        List<PileView> pilesToScan = new List<PileView>(_tableauPiles);
        List<CardView> wasteCards = new List<CardView>(_wastePile.GetCardsInPile());

        while (!isWon)
        {
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
                    yield return new WaitForSeconds(0.1f); 
                    break;
                }
            }


            if (!movedCardInThisLoop) break;

            isWon = WinValidator.CheckForVictory(_foundationPiles);

        }

        
        if (isWon)
        {
            OnAutoCompleteFinished?.Invoke();
        }
    }

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
}