using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Card
{
    public Suit suit;
    public int val;
    public Sprite face;
    Image image;
    Transform transform;
    GameObject go;

    public Card(Suit suit, int val, Sprite face, GameObject go)
    {
        this.suit = suit;
        this.val = val;
        this.face = face;
        this.transform = go.transform;
        this.image = go.GetComponent<Image>();
        this.go = go;
        UpdateCard(face);
    }
    public void UpdateCard(Sprite face)
    {
        image.sprite = face;   
    }
    public void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public GameObject GetGameObject()
    {
        return go;
    }

}
