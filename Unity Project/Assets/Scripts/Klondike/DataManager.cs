using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klondike.Commands;

namespace Klondike
{
    public class DataManager : MonoBehaviour
    {
        // Prefab do modelo de carta
        [SerializeField]
        Transform cardPrefab;
        // Linkando as pilhas de baixo e as de cima
        public Transform[] bottomPilesObjects;
        public Transform[] topPilesObjects;
        public Transform trioObject;
        public Vector3[] trioPositions;
        public Vector3 cardScale;
        public SpriteRenderer DeckSpriteRenderer;

        // Monte de cartas, inicialmente contém todas as cartas
        public List<int> deck { get; private set; }
        // Trio de cartas visíveis (acionados pelo monte)
        public List<int> newDeck { get; private set; }
        // Descarte (reposto ao monte)

        public int[] trio;
        // Trio visível

        public Stack<Command> commandHistory { get; private set; }

        public List<int>[] bottomPiles { get; private set; }
        // Contém todas as pilhas de baixo

        public List<int>[] topPiles { get; private set; }
        // Contém todas as pilhas de cima

        public int LastCardInDeck
        {
            get => deck[deck.Count - 1];
        }

        public void Initialize()
        {
            deck = new List<int>();
            newDeck = new List<int>();
            trio = new int[3] { -1, -1, -1 };
            bottomPiles = new List<int>[7];
            for (int i = 0; i < 7; i++)
            {
                bottomPiles[i] = new List<int>();
            }
            topPiles = new List<int>[4];
            for (int i = 0; i < 4; i++)
            {
                topPiles[i] = new List<int>();
            }
            commandHistory = new Stack<Command>();
        }
    
        public void GenerateDeck()
        {
            List<int> generatedDeck = new List<int>();
            for (int i = 0; i < 52; i++)
            {
                generatedDeck.Add(i);
            }
            deck = generatedDeck;
        }
    
        public void ShuffleDeck()
        {
            System.Random rng = new System.Random();
            int n = 52;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }
    
        public Transform InstantiateCard(int card, bool visible)
        {
            Transform newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, null);
            newCard.name = card.ToString();
            newCard.GetComponent<CardManager>().InitializeCard(card, visible);
            newCard.localScale = cardScale;
            return (newCard);
        }

        // Move card functions

        public int MoveCardIntoTrio(Transform card, bool left, bool newCard = false, int cardValue = 0)
        {
            CardManager cardManager = card.GetComponent<CardManager>();
            int i = 0;
            int shifted = 0;

            if (left)
            {
                if (trio[0] != -1)
                {
                    shifted++;
                    ShiftTrio(false);
                }

                if (!newCard)
                {
                    if (cardManager.bottom)
                    {
                        trio[0] = bottomPiles[cardManager.pile][cardManager.position];
                        bottomPiles[cardManager.pile].RemoveAt(cardManager.position);
                    }
                    else
                    {
                        trio[0] = topPiles[cardManager.group][topPiles[cardManager.group].Count - 1];
                        topPiles[cardManager.group].RemoveAt(topPiles[cardManager.group].Count - 1);
                    }
                }
                else
                {
                    trio[0] = cardValue;
                }
            }
            else
            {
                i = 2;
                if (trio[2] != -1)
                {
                    shifted--;
                    ShiftTrio();
                }
                else
                {
                    for (; (i >= 0) && (trio[i] == -1); i--) ;
                    i++;
                }

                if (!newCard)
                {
                    if (cardManager.bottom)
                    {
                        trio[i] = bottomPiles[cardManager.pile][cardManager.position];
                        bottomPiles[cardManager.pile].RemoveAt(cardManager.position);
                    }
                    else
                    {
                        trio[i] = topPiles[cardManager.group][topPiles[cardManager.group].Count - 1];
                        topPiles[cardManager.group].RemoveAt(topPiles[cardManager.group].Count - 1);
                    }
                }
                else
                {
                    trio[i] = cardValue;
                }
            }
            
            card.parent = trioObject;
            card.localPosition = trioPositions[i];
            card.SetSiblingIndex(i);
            cardManager.trio = true;
            return (shifted);
        }

        public void MoveCardIntoTopPile(Transform card, int targetPileNumber)
        {
            CardManager cardManager = card.GetComponent<CardManager>();
            Transform targetPileObject = topPilesObjects[targetPileNumber];
            int targetPileCount = targetPileObject.childCount;
            Vector3 referencePos;
            if (targetPileCount == 0)
            {
                referencePos = targetPileObject.position;
            }
            else
            {
                referencePos = targetPileObject.GetChild(targetPileCount - 1).position;
            }
            referencePos.z -= 0.03f;
            card.position = referencePos;
            if (cardManager.trio)
            {
                topPiles[targetPileNumber].Add(trio[card.GetSiblingIndex()]);
                trio[card.GetSiblingIndex()] = -1;
            }
            else
            {
                topPiles[targetPileNumber].Add(bottomPiles[cardManager.pile][bottomPiles[cardManager.pile].Count - 1]);
                bottomPiles[cardManager.pile].RemoveAt(bottomPiles[cardManager.pile].Count - 1);
            }
            card.parent = targetPileObject;
            cardManager.top = true;
        }

        public void MoveCardIntoBottomPile(Transform card, int targetPileNumber, bool newCard = false, int cardValue = 0)
        {
            CardManager cardManager = card.GetComponent<CardManager>();
            Transform targetPileObject = bottomPilesObjects[targetPileNumber];
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
                if (cardManager.trio)
                {
                    bottomPiles[targetPileNumber].Add(trio[card.GetSiblingIndex()]);
                    trio[card.GetSiblingIndex()] = -1;
                }
                else if (cardManager.top)
                {
                    bottomPiles[targetPileNumber].Add(topPiles[cardManager.group][topPiles[cardManager.group].Count - 1]);
                    topPiles[cardManager.group].RemoveAt(topPiles[cardManager.group].Count - 1);
                }
                else
                {
                    bottomPiles[targetPileNumber].Add(bottomPiles[cardManager.pile][cardManager.position]);
                    bottomPiles[cardManager.pile].RemoveAt(cardManager.position);
                }
            }
            else
            {
                bottomPiles[targetPileNumber].Add(cardValue);
                cardManager.pile = targetPileNumber;
            }
            card.parent = targetPileObject;
        }
        
        public void MoveDescendingPileIntoBottomPile(Transform card, int size, int targetPile)
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
        
        public void ResetDeck()
        {
            int i;
            // Destroy trio
            if (trioObject.childCount > 0)
            {
                i = 0;
                int trioCount = trioObject.childCount;

                while (i < trioCount)
                {
                    newDeck.Add(trio[i]);
                    Destroy(trioObject.GetChild(i).gameObject);
                    trio[i] = -1;
                    i++;
                }

                // Restack the deck
                deck = newDeck;
                deck.Reverse();
                newDeck = new List<int>();
                DeckSpriteRenderer.sprite = FindObjectOfType<SpriteController>().availableSprites[53];
            }
        }
        
        public void UndoResetDeck()
        {
            int baseIndex = deck.Count - 3;
            DeckSpriteRenderer.sprite = FindObjectOfType<SpriteController>().availableSprites[54];
            deck.Reverse();

            for (int i = 0; (i < 3) && (i < deck.Count); i++)
            {
                Transform instantiatedCard = InstantiateCard(deck[baseIndex], true);
                MoveCardIntoTrio(instantiatedCard, false, true, deck[baseIndex]);
                deck.RemoveAt(baseIndex);
            }

            newDeck = deck;
            deck = new List<int>();
        }

        public void ShiftTrio(bool left = true)
        {
            GameObject destroyed;
            int trioCount = trioObject.childCount;

            if (left)
            {
                if (trio[0] != -1)
                {
                    destroyed = trioObject.GetChild(0).gameObject;
                    destroyed.transform.parent = null;
                    Destroy(destroyed);
                    newDeck.Add(trio[0]);
                }

                trio[0] = trio[1];
                trio[1] = trio[2];
                trio[2] = -1;
                trioObject.GetChild(0).localPosition = trioPositions[0];
                trioObject.GetChild(1).localPosition = trioPositions[1];
            }
            else
            {
                if (trio[2] != -1)
                {
                    destroyed = trioObject.GetChild(2).gameObject;
                    destroyed.transform.parent = null;
                    Destroy(destroyed);
                    deck.Add(trio[2]);
                }

                trio[2] = trio[1];
                trio[1] = trio[0];
                trio[0] = -1;
                trioObject.GetChild(0).localPosition = trioPositions[1];
                trioObject.GetChild(1).localPosition = trioPositions[2];
            }
        }

        public bool IsCardInTrioBlocked(Transform card)
        {
            if (card.parent.GetChild(card.parent.childCount - 1) == card)
            {
                return (false);
            }

            return (true);
        }

        public int DescendingPileSize(Transform card)
        {
            Transform parentPile = card.parent;
            Transform c1;
            Transform c2;

            int basePilePosition = card.GetComponent<CardManager>().position;
            int i = basePilePosition;
            int limit = parentPile.childCount - 1;

            while (i < limit)
            {
                c1 = parentPile.GetChild(i);
                i++;
                c2 = parentPile.GetChild(i);

                CardManager m1 = c1.GetComponent<CardManager>();
                CardManager m2 = c2.GetComponent<CardManager>();

                if (!((m1.isRed ^ m2.isRed) && (m1.value == m2.value + 1)))
                {
                    break;
                }
            }

            if (i == limit)
            {
                i += 1 - basePilePosition;
                return (i);
            }

            return (-1);
        }

        public bool SetPreviousCardVisibility(Transform card, bool visibility = true)
        {
            int previousCardPos = card.GetSiblingIndex() - 1;
            if (previousCardPos >= 0)
            {
                bool aux = card.parent.GetChild(previousCardPos).GetComponent<CardManager>().visible;
                card.parent.GetChild(previousCardPos).GetComponent<CardManager>().visible = visibility;
                return (aux ^ visibility);
            }

            return (false);
        }
    }
}
