using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Tooltip("Add cards to be displayed in the UI")]
    [SerializeField] Card[] cards;

    [Tooltip("Card container")]
    [SerializeField] Transform container;
    [Tooltip("Base Card prefab")]
    [SerializeField] GameObject cardPrefab;
    Dictionary<Suit, List<Card>> renderCards = new Dictionary<Suit, List<Card>>();

    #region card view
    [Tooltip("Card View")]
    [SerializeField] RectTransform cardView;
    [SerializeField] int cardsToDisplay = 4;
    #endregion

    #region card pool
    int deckSize = 52;
    Queue<GameObject> cardPool = new Queue<GameObject>();
    GameObject spawnGO;
    #endregion

    List<Card> tempList;
    Card tempCard;

    void Start()
    {
        SetCardViewSize();
        GenerateCardPool();
        GenerateCards();
    }

    private void SetCardViewSize()
    {
        HorizontalLayoutGroup layoutGroup = container.GetComponent<HorizontalLayoutGroup>();
        float baseWidth = cardPrefab.GetComponent<RectTransform>().rect.width * cardsToDisplay;
        float spacing = layoutGroup.spacing * (cardsToDisplay - 1);
        float padding = layoutGroup.padding.left + layoutGroup.padding.right; 
        cardView.sizeDelta = new Vector2(baseWidth + spacing + padding, cardView.rect.height);
    }

    void GenerateCards()
    {
        // We begin by iterating through our cards array, which is populated in the editor...
        for (int i = 0; i < cards.Length; i++)
        {
            // ... then generate a new card data object with a reference to its instantiated cardPrefab
            tempCard = new Card(cards[i].suit, cards[i].val, cards[i].face, DrawFromPool());

            // Then we check if this card's suit has been processed before or not, and we add to our renderCard dictionary
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

        // Our suit order is determined by the value order in the Suit enum
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            try
            {
                if (renderCards.TryGetValue(suit, out List<Card> cardList))
                {
                    // Once in our suit, we sort our cardList by card value and set to our parent container
                    tempList = cardList.OrderBy(card => card.val).ToList();
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        // Our parent's content resizer and horizontal layout components will ensure our scrollable area is laid out properly and expanded to fit our spawned cards
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
    void GenerateCardPool()
    {
        for (int i = 0; i < deckSize; i++)
        {
            spawnGO = Instantiate(cardPrefab);
            AddToPool(spawnGO);
        }
    }
    void AddToPool(GameObject toAdd)
    {
        spawnGO.transform.SetParent(transform);
        spawnGO.SetActive(false);
        cardPool.Enqueue(spawnGO);
    }
    GameObject DrawFromPool()
    {
        spawnGO = cardPool.Dequeue();
        spawnGO.SetActive(true);
        return spawnGO;
    }
}
