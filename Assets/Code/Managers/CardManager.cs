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
    Dictionary<Transform, Card> renderCards = new Dictionary<Transform, Card>();

    #region card view
    [Tooltip("Card View")]
    [SerializeField] RectTransform cardView;
    [SerializeField] int cardsToDisplay = 4;
    [SerializeField] float spacing;
    float cardWidth = 100;
    #endregion
    int cardIndex;

    #region card pool
    int poolSize;

    Queue<GameObject> cardPool = new Queue<GameObject>();
    GameObject spawnGO;
    #endregion

    List<Card> tempList;
    [SerializeField] List<GameObject> cardList = new List<GameObject>();
    Card tempCard;
    List<Card> sortedCards;
    Vector3 newPos;
    Transform checkCard;

    float lastX = 0;
    float moveDir = 1;

    int currentIndex = 0;

    void Start()
    {
        SetCardViewSize();
        SortCards();
        GenerateCards();
    }
    
    private void SetCardViewSize()
    {
        float baseWidth = cardsToDisplay * cardWidth;
        cardView.sizeDelta = new Vector2(baseWidth , cardView.rect.height);
        container.sizeDelta = cardView.sizeDelta;
    }

    void SortCards()
    {
        sortedCards = new List<Card>();
        Dictionary<Suit, List<Card>> cardsBySuit = new Dictionary<Suit, List<Card>>();
        for (int i = 0; i < cards.Length; i++)
        {
            if (cardsBySuit.TryGetValue(cards[i].suit, out List<Card> list))
            {
                list.Add(cards[i]);
                cardsBySuit[cards[i].suit] = list;
            }
            else
            {
                tempList = new List<Card>();

                tempList.Add(cards[i]);
                cardsBySuit[cards[i].suit] = tempList;
            }
        }

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {

            if (cardsBySuit.TryGetValue(suit, out List<Card> list))
            {
                tempList = list.OrderBy(card => card.val).ToList();
                for (int i = 0; i < tempList.Count; i++)
                {
                    sortedCards.Add(tempList[i]);
                }
            }
            else
            {
                Debug.Log($"No cards found for suit: {suit}");
            }            
        }
    }

    void GenerateCards()
    {
        poolSize = cardsToDisplay + 2;
        for (int i = 0; i < sortedCards.Count; i++)
        {
            if (i >= poolSize) break;
            tempCard = new Card(sortedCards[i].suit, sortedCards[i].val, sortedCards[i].face, GenerateCard("Card " + (i + 1)));
            currentIndex = i;
            tempCard.SetParent(container);
            cardList.Add(tempCard.GetGameObject());

            newPos = tempCard.GetGameObject().transform.position;
            newPos.x += (cardList.Count * cardWidth * 1.5f) + cardList.Count * spacing;
            tempCard.SetPosition(newPos);

            renderCards[tempCard.GetGameObject().transform] = tempCard;
        }
    }

    GameObject GenerateCard(string name)
    {
        spawnGO = new GameObject(name);
        Image image = spawnGO.AddComponent<Image>();
        image.preserveAspect = true;
        image.rectTransform.sizeDelta = new Vector2(cardWidth, image.rectTransform.rect.height);
        return spawnGO;
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
                cardIndex = currentIndex + 1 < sortedCards.Count ? currentIndex + 1 : 0;
                renderCards[checkCard].UpdateCard(sortedCards[cardIndex].face);
                currentIndex = cardIndex;

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
                cardIndex = currentIndex - poolSize >= 0 ? currentIndex - poolSize : sortedCards.Count + (currentIndex - poolSize);
                renderCards[checkCard].UpdateCard(sortedCards[cardIndex].face);
                currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : sortedCards.Count - 1;

                newPos = container.GetChild(0).position;
                newPos.x -= (cardWidth * 1.5f) + spacing;
                checkCard.position = newPos;
                checkCard.transform.SetAsFirstSibling();
            }
        }
    }
}
