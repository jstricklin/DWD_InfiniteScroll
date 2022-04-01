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
    [SerializeField] RectTransform container;
    [Tooltip("Base Card prefab")]
    //[SerializeField] GameObject cardPrefab;
    Dictionary<Suit, List<Card>> renderCards = new Dictionary<Suit, List<Card>>();

    #region card view
    [Tooltip("Card View")]
    [SerializeField] RectTransform cardView;
    [SerializeField] int cardsToDisplay = 4;
    [SerializeField] float spacing;
    float cardWidth = 100;
    #endregion

    #region card pool
    int deckSize = 52;
    Queue<GameObject> cardPool = new Queue<GameObject>();
    GameObject spawnGO;
    #endregion

    List<Card> tempList;
    [SerializeField] List<GameObject> cardList = new List<GameObject>();
    Card tempCard;

    Vector3 newPos;
    Transform checkCard;

    float lastX = 0;
    float moveDir = 1;

    void Start()
    {
        SetCardViewSize();
        GenerateCardPool();
        GenerateCards();
    }
    
    private void SetCardViewSize()
    {
        //Here we dynamically resize our card display area to ensure we only display the desired amount of cards
        
        float baseWidth = cardsToDisplay * cardWidth;
        cardView.sizeDelta = new Vector2(baseWidth , cardView.rect.height);
    }
    void GenerateCardPool()
    {
        // Here we generate a pool of cards based on a max deckSize
        for (int i = 0; i < deckSize; i++)
        {
            spawnGO = new GameObject("Card " + i);
            Image image = spawnGO.AddComponent<Image>();
            image.preserveAspect = true;
            image.rectTransform.sizeDelta = new Vector2(cardWidth, image.rectTransform.rect.height);
            AddToPool(spawnGO);
        }
    }
    void AddToPool(GameObject toAdd)
    {
        // Here we add (or could return) card GameObjects to the cardPool
        spawnGO.transform.SetParent(transform);
        spawnGO.SetActive(false);
        cardPool.Enqueue(spawnGO);
    }
    GameObject DrawFromPool()
    {
        // Here we draw cards from the cardPool
        spawnGO = cardPool.Dequeue();
        spawnGO.SetActive(true);
        return spawnGO;
    }
    void GenerateCards()
    {
        // We begin by iterating through our cards array, which is populated in the editor...
        for (int i = 0; i < cards.Length; i++)
        {
            // ... then generate a new card data object and draw a blank card from our card pool
            tempCard = new Card(cards[i].suit, cards[i].val, cards[i].face, DrawFromPool());

            // Then we check if this card's suit has been processed before or not, and we add to our renderCard dictionary
            if (renderCards.TryGetValue(cards[i].suit, out List<Card> list))
            {
                list.Add(tempCard);
                renderCards[cards[i].suit] = list;
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
                if (renderCards.TryGetValue(suit, out List<Card> list))
                {
                    // Once in our suit, we sort our cardList by card value and set to our parent container
                    tempList = list.OrderBy(card => card.val).ToList();
                    for (int i = 0; i < tempList.Count; i++)
                    {

                        tempList[i].SetParent(container);
                        cardList.Add(tempList[i].GetGameObject());

                        newPos = tempList[i].GetGameObject().transform.position;
                        
                        newPos.x += (cardList.Count * cardWidth * 1.5f) + cardList.Count * spacing;

                        tempList[i].SetPosition(newPos);

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

    public void ValueChanged(Vector2 vector)
    {
        moveDir = (container.position.x - lastX) > 0 ? 1 : -1;
        lastX = container.position.x;
        HandleCardScroll(moveDir);
    }

    private void HandleCardScroll(float dir)
    {
        if (dir < 0)
        {

            checkCard = container.GetChild(0);
            if (checkCard.position.x + cardWidth * 2 < cardView.position.x - cardView.rect.width)
            {
                newPos = container.GetChild(container.childCount - 1).position;
                newPos.x += (cardWidth * 1.5f) + spacing;
                checkCard.position = newPos;
                checkCard.transform.SetAsLastSibling();
            }
        } else
        {
            checkCard = container.GetChild(container.childCount - 1);
            if (checkCard.position.x - cardWidth * 2 > cardView.position.x + cardView.rect.width)
            {

                newPos = container.GetChild(0).position;
                newPos.x -= (cardWidth * 1.5f) + spacing;
                checkCard.position = newPos;
                checkCard.transform.SetAsFirstSibling();
            }
        }
    }
}
