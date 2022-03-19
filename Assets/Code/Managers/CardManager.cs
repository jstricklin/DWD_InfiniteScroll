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
        // sort card order
        for (int i = 0; i < cards.Length; i++)
        {

        }

            // generate cards
            for (int i = 0; i < cards.Length; i++)
        {
            tempCard = new Card(cards[i].suit, cards[i].val, cards[i].face, Instantiate(cardPrefab));
            tempCard.SetParent(container);
            
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
    }
}
