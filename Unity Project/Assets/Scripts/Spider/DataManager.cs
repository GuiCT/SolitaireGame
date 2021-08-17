using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spider.Commands;

namespace Spider
{
    public class DataManager : MonoBehaviour
    {
        // Prefab do modelo de carta
        [SerializeField]
        Transform cardPrefab;
        public Transform finishedGroups;
        public Transform deckButton;
        // Linkando as pilhas de cima
        public Transform[] pilesObjects;

        // Monte de cartas, inicialmente contém todas as cartas
        public List<int> deck;
        // Contém todas as pilhas de cima
        public List<int>[] piles { get; private set; }
        public int LastCard
        {
            get => deck[deck.Count - 1];
        }
        public bool EmptyDeck
        {
            get => deck.Count == 0;
        }
        public Stack<Command> commandHistory { get; private set; }

        public void Initialize()
        {
            piles = new List<int>[10];
            for (int i = 0; i < 10; i++)
            {
                piles[i] = new List<int>();
            }
            commandHistory = new Stack<Command>();
        }
        public void GenerateDeck()
        {
            List<int> newDeck = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    newDeck.Add(i * 13 + j);
                }
            }

            deck = newDeck;
        }
        public void ShuffleDeck()
        {
            System.Random rng = new System.Random();
            int n = 104;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        public void MoveCardIntoPile(Transform card, int targetPileNumber, bool newCard = false, int cardValue = 0)
        {
            CardManager cardManager = card.GetComponent<CardManager>();
            Transform targetPileObject = pilesObjects[targetPileNumber];
            Vector3 referencePos;
            int targetPileSize = targetPileObject.childCount;
            if (targetPileSize == 0)
            {
                referencePos = targetPileObject.position;
            }
            else
            {
                referencePos = targetPileObject.GetChild(targetPileSize - 1).position;
                referencePos.y -= 0.3f;
            }
            referencePos.z -= 0.03f;
            card.position = referencePos;
            if (!newCard)
            {
                piles[targetPileNumber].Add(piles[cardManager.pile][cardManager.position]);
                piles[cardManager.pile].RemoveAt(cardManager.position);
            }
            else
            {
                piles[targetPileNumber].Add(cardValue);
                cardManager.pile = targetPileNumber;
            }

            card.parent = targetPileObject;
            card.localScale = Vector3.one;
        }

        public Transform InstantiateCard(int card, bool visible)
        {
            Transform newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, null);
            newCard.name = card.ToString();
            newCard.GetComponent<CardManager>().InitializeCard(card, -1, visible);
            return (newCard);
        }

        public void MoveDescendingPile(Transform card, int size, int targetPile)
        {
            Transform parentPile = card.parent;
            int currentIndex = card.GetSiblingIndex();
            for (int i = 0; i < size; i++)
            {
                parentPile.GetChild(currentIndex).GetComponent<CardManager>().pile = targetPile;
            }
        }

        public void SetSelectionOfDescendingPile(Transform card, int size, bool selected)
        {
            Transform pile = card.parent;
            int baseIndex = card.GetSiblingIndex();
            for (int i = 0; i < size; i++)
            {
                pile.GetChild(baseIndex + i).GetComponent<CardManager>().selected = selected;
            }
        }

        public int DescendingPileSize(Transform card)
        {
            Transform parentPile = card.parent;
            Transform c1;
            Transform c2;

            int basePilePosition = card.GetSiblingIndex();
            int i = basePilePosition;
            int limit = parentPile.childCount - 1;

            while (i < limit)
            {
                c1 = parentPile.GetChild(i);
                c2 = parentPile.GetChild(i + 1);

                CardManager m1 = c1.GetComponent<CardManager>();
                CardManager m2 = c2.GetComponent<CardManager>();

                if (!(m1.value == m2.value + 1))
                {
                    break;
                }

                i++;
            }

            if (i == limit)
            {
                i = i + 1 - basePilePosition;
                return (i);
            }

            return (-1);
        }

        // Vira a carta anterior CASO ELA EXISTA
        public bool SetPreviousCardVisibility(Transform card, bool visibility = true)
        {
            if (card.GetSiblingIndex() - 1 >= 0)
            {
                Transform turnedCard = card.parent.GetChild(card.GetSiblingIndex() - 1);

                CardManager turnedCardManager = turnedCard.GetComponent<CardManager>();
                bool aux = turnedCardManager.visible;
                turnedCardManager.visible = visibility;
                return (aux ^ visibility);
            }
            else
            {
                return (false);
            }
        }

        public bool PileHasCompleteGroup(int pileIndex)
        {
            int pileSize = piles[pileIndex].Count;
            if (pileSize >= 13)
            {
                int groupSize = DescendingPileSize(pilesObjects[pileIndex].GetChild(pileSize - 13));
                if (groupSize == 13)
                {
                    return (true);
                }
            }

            return (false);
        }
    }
}