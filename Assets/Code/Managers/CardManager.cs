using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] Card[] cards;
    [SerializeField] RectTransform container;
    [SerializeField] Dictionary<Suit, List<Card>> renderCards = new Dictionary<Suit, List<Card>>();
    [SerializeField] GameObject cardPrefab;
    List<Card> tempList;
    Card tempCard;

    // Start is called before the first frame update
    void Start()
    {
        InitializeCards();
    }

    void InitializeCards()
    {
        // generate cards
        for (int i = 0; i < cards.Length; i++)
        {
            tempCard = new Card(cards[i].suit, cards[i].val, cards[i].face, Instantiate(cardPrefab));
            
            if (renderCards.TryGetValue(cards[i].suit, out List<Card> cardList))
            {
                cardList.Add(tempCard);
                renderCards[cards[i].suit] = cardList;
            } else
            {
                tempList = new List<Card>();

                tempList.Add(tempCard);
                renderCards[cards[i].suit] = tempList;
            }
        }
        // organize cards and set cards to parent container
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            try
            {
                if (renderCards.TryGetValue(suit, out List<Card> cardList))
                {
                    tempList = cardList.OrderBy(a => a.val).ToList();
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        tempList[i].SetParent(container);
                    }
                }
                else
                {
                    throw new Exception($"No cards found for suit: {suit}");
                }
            } catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}
