using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Card
{
    public Suit suit;
    public int val;
    public Sprite face;
    Transform transform;

    public Card(Suit suit, int val, Sprite face, GameObject go)
    {
        this.suit = suit;
        this.val = val;
        this.face = face;
        this.transform = go.transform;
        go.GetComponent<Image>().sprite = face;
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
    }

}
