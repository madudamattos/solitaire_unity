using UnityEngine;
using UnityEditor;
using Solitaire.Core;
using Solitaire.Models;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

[CustomEditor(typeof(DeckData))]
public class DeckDataEditor : Editor
{
    private Texture2D _spriteSheet;

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Deck Generation Settings", EditorStyles.boldLabel);
        
        _spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Texture (Sprite Sheet)", _spriteSheet, typeof(Texture2D), false);

        if (GUILayout.Button("Auto-populate Deck"))
        {
            FillDeck((DeckData)target);
        }

        GUILayout.Space(10);

        base.OnInspectorGUI(); // Desenha o inspetor padrão
    }

    private void FillDeck(DeckData data)
    {
        if (_spriteSheet == null) return;

        // Carrega todos os sprites que estão dentro da textura
        string path = AssetDatabase.GetAssetPath(_spriteSheet);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> sprites = assets.OfType<Sprite>().OrderBy(s => {
            string prefix = s.name.Substring(0,4);
            string card_number = s.name.Substring(s.name.LastIndexOf("_") + 1);

            int.TryParse(card_number, out int number);

            int priority = prefix == "Back" ? 0 : 1000; 

            return priority + number;
        }).ToList();
        
        foreach(Sprite s in sprites)
        {
            Debug.Log(s.name);   
        }

        if (data.cards != null)
        {
            data.cards.Clear();
            data.cardsBack.Clear();   
        }
        else
        {
            data.cards = new List<CardVisualData>();
            data.cardsBack = new List<CardBackData>();   
        }

        int spriteIndex = 0; 
        
        foreach (Back b in System.Enum.GetValues(typeof(Back)))
        {
            if (spriteIndex >= sprites.Count) break;

            CardBackData cardBack;
            cardBack.color = b;
            cardBack.cardBackSprite = sprites[spriteIndex];
            data.cardsBack.Add(cardBack);

            spriteIndex++;
        }

        Debug.Log("First sprite index:" + spriteIndex);

        Debug.Log("Suit values:" + System.Enum.GetValues(typeof(Suit)).Length);
        Debug.Log("Rank values:" + System.Enum.GetValues(typeof(Rank)).Length);

        foreach (Suit s in System.Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank r in System.Enum.GetValues(typeof(Rank)))
            {
                if (spriteIndex >= sprites.Count) break;

                CardVisualData card;
                card.suit = s;
                card.rank = r;
                card.cardSprite = sprites[spriteIndex];
                data.cards.Add(card);

                spriteIndex++;
            }
        }


        EditorUtility.SetDirty(data); // Avisa a Unity que o SO mudou e precisa salvar
        AssetDatabase.SaveAssets();
        Debug.Log("Deck generated successfully!");
    }
}