using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Probability : MonoBehaviour
{

    [SerializeField] int checkTimes = 2;

    [System.Serializable]
    class Drawer
    {
        [SerializeField] Socks[] _socks;
        public List<Sock> socks = new List<Sock>();

        public void GenerateSocks()
        {
            socks.Clear();
            for (int i = 0; i < _socks.Length; i++)
            {
                for (int s = 0; s < _socks[i].amount; s++)
                {
                    socks.Add(new Sock(_socks[i].color));
                }
            }
        }
        public Sock GetSockFromDrawer()
        {
            int randomIndex = Random.Range(0, socks.Count);
            Sock toRemove = socks[randomIndex];
            socks.Remove(toRemove);
            return toRemove;
        }

        public (Sock sockA, Sock sockB) getSockPair()
        {
            (Sock sockA, Sock sockB) pair = (GetSockFromDrawer(), GetSockFromDrawer());
            // Refresh socks after each pair 
            GenerateSocks();
            return pair;
        }
    }

    [SerializeField]
    Drawer drawer;

    [System.Serializable]
    struct Socks
    {
        public Color color;
        public int amount;
    }

    struct Sock
    {
        public Color color;
        public Sock(Color color)
        {
            this.color = color;
        }
    }

    private void Start()
    {
        drawer.GenerateSocks();
        Debug.Log("Press 'Spacebar' to run the Probability test");
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log($"Probability to match ({checkTimes}x) : {ProbabilityToMatch(checkTimes)}");
        }
    }

    bool isPair(Drawer drawer)
    {
        (Sock sockA, Sock sockB) pair = drawer.getSockPair();
        return pair.sockA.color.Equals(pair.sockB.color);
    }

    float ProbabilityToMatch(int count)
    {
        int pairs = 0;
        for (int i = 0; i < count; i++)
        {
            pairs += isPair(drawer) ? 1 : 0;
        }
        return (float)pairs / (float)count;
    }
}
