using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Klondike.Commands
{
    public class UseDeck : Command
    {
        bool shiftedTrio;
        bool reseted;

        public override void Execute()
        {
            if (dataManager.deck.Count > 0)
            {
                reseted = false;
                int cardValue = dataManager.LastCardInDeck;
                Transform instantiatedCard = dataManager.InstantiateCard(cardValue, true);
                shiftedTrio = dataManager.MoveCardIntoTrio(instantiatedCard, false, true, cardValue) == -1;
                dataManager.deck.RemoveAt(dataManager.deck.Count - 1);
                if (dataManager.deck.Count == 0)
                {
                    dataManager.DeckSpriteRenderer.sprite = Object.FindObjectOfType<SpriteController>().availableSprites[54];
                }
            }
            else
            {
                reseted = true;
                dataManager.ResetDeck();
            }
        }

        public override void Undo()
        {
            if (reseted)
            {
                dataManager.UndoResetDeck();
            }
            else
            {
                if (dataManager.deck.Count == 0)
                {
                    dataManager.DeckSpriteRenderer.sprite = Object.FindObjectOfType<SpriteController>().availableSprites[53];
                }
                if (shiftedTrio)
                {
                    int cardValue = dataManager.newDeck[dataManager.newDeck.Count - 1];
                    Transform instantiatedCard = dataManager.InstantiateCard(cardValue, true);
                    dataManager.MoveCardIntoTrio(instantiatedCard, true, true, cardValue);
                    dataManager.newDeck.RemoveAt(dataManager.newDeck.Count - 1);
                }
                else
                {
                    int trioCount = dataManager.trioObject.childCount;
                    GameObject destroyed = dataManager.trioObject.GetChild(trioCount - 1).gameObject;
                    dataManager.deck.Add(dataManager.trio[destroyed.transform.GetSiblingIndex()]);
                    dataManager.trio[destroyed.transform.GetSiblingIndex()] = -1;
                    destroyed.transform.parent = null;
                    Object.Destroy(destroyed);
                }
            }
        }
    }
}
