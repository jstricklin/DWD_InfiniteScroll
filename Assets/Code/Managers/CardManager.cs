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
    [SerializeField] float spacing = 20f;
    [SerializeField] float padding = 20f;
    float cardWidth;
    #endregion

    #region card pool
    int deckSize = 52;
    Queue<GameObject> cardPool = new Queue<GameObject>();
    GameObject spawnGO;
    #endregion

    List<Card> tempList;
    [SerializeField] List<GameObject> cardList = new List<GameObject>();
    Card tempCard;

    void Start()
    {

        GenerateCardPool();
        SetCardViewSize();
        GenerateCards();
    }

    float lastX = 0;
    float moveDir = 1;
    public void ValueChanged(Vector2 vector)
    {
        //Debug.Log("val check: " + container.position.x);
        moveDir = (container.position.x - lastX) > 0 ? 1 : -1;
        lastX = container.position.x;
        HandleCardScroll(moveDir);
    }
    Vector3 newPos;
    Transform checkCard;
    private void HandleCardScroll(float dir)
    {
        Debug.Log("check " + dir);
        if (dir < 0)
        {

            checkCard = container.GetChild(0);
            //Debug.Log($"transform checks : {(checkCard.position.x)} | {cardView.rect.width}");
            if (checkCard.position.x + cardWidth * 2f < cardView.rect.width)
            {
                newPos = container.GetChild(container.childCount - 1).position;
                newPos.x += (cardWidth * 2f) + spacing;
                checkCard.position = newPos;
                checkCard.transform.SetAsLastSibling();
            }
        } else
        {
            checkCard = container.GetChild(container.childCount - 1);
            Debug.Log($"transform checks : {(checkCard.position.x)} | {cardView.rect.width}");
            if (checkCard.position.x - cardWidth * 2f > cardView.rect.width * 3)
            {
                //Debug.Log("CHECK 2");

                newPos = container.GetChild(0).position;
                newPos.x -= (cardWidth * 2f) + spacing;
                checkCard.position = newPos;
                checkCard.transform.SetAsFirstSibling();
            }
        }
    }

    private void SetCardViewSize()
    {
        //Here we dynamically resize our card display area to ensure we only display the desired amount of cards
        HorizontalLayoutGroup layoutGroup = container.GetComponent<HorizontalLayoutGroup>();
        //float baseWidth = 100 * cardsToDisplay;
        //float spacing = layoutGroup.spacing * (cardsToDisplay - 1);
        //float padding = layoutGroup.padding.left + layoutGroup.padding.right;
        float baseWidth = cardsToDisplay * cardWidth;
        //cardView.sizeDelta = new Vector2(baseWidth + spacing + padding, cardView.rect.height);
        cardView.sizeDelta = new Vector2(baseWidth + (spacing * (cardsToDisplay - 1)) + (padding * 2), cardView.rect.height);
    }
    void GenerateCardPool()
    {
        // Here we generate a pool of cards based on a max deckSize
        for (int i = 0; i < deckSize; i++)
        {
            //spawnGO = Instantiate(cardPrefab);
            spawnGO = new GameObject("Card " + i);
            spawnGO.AddComponent<Image>().preserveAspect = true;
            cardWidth = spawnGO.GetComponent<Image>().rectTransform.rect.width;
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
                        newPos = tempList[i].GetGameObject().transform.position;

                        newPos.x += padding + (cardList.Count * (cardWidth * 2f + spacing * 2));
                        Debug.Log("check: " + newPos.x);
                        cardList.Add(tempList[i].GetGameObject());
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
}
