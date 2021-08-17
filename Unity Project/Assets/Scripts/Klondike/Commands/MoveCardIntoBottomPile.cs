using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klondike.Commands;

namespace Klondike.Commands
{
    public class MoveCardIntoBottomPile : Command
    {
        CardManager cardManager;
        Transform cardReference;
        CardPosition originalPos;
        int originalPile;
        int targetPile;
        int cardsMoved;
        bool shiftedTrio;
        bool previousTurned;

        public MoveCardIntoBottomPile(Transform cardReference, int targetPile) : base()
        {
            this.cardReference = cardReference;
            this.targetPile = targetPile;
            cardManager = cardReference.GetComponent<CardManager>();
            originalPos = CardPosition.TRIO;
            originalPile = -1;
            cardsMoved = 1;
        }

        public MoveCardIntoBottomPile(Transform cardReference, int group, int targetPile) : base()
        {
            this.cardReference = cardReference;
            this.targetPile = targetPile;
            cardManager = cardReference.GetComponent<CardManager>();
            originalPos = CardPosition.TOP_PILE;
            originalPile = group;
            cardsMoved = 1;
        }

        public MoveCardIntoBottomPile(Transform cardReference, int originalPile, int targetPile, int cardsMoved) : base()
        {
            this.cardReference = cardReference;
            this.originalPile = originalPile;
            this.targetPile = targetPile;
            this.cardsMoved = cardsMoved;
            cardManager = cardReference.GetComponent<CardManager>();
            originalPos = CardPosition.BOTTOM_PILE;
        }

        public override void Execute()
        {
            if (originalPos == CardPosition.BOTTOM_PILE)
            {
                previousTurned = dataManager.SetPreviousCardVisibility(cardReference, true);
                dataManager.MoveDescendingPileIntoBottomPile(cardReference, cardsMoved, targetPile);
            }
            else
            {
                cardManager.pile = targetPile;
            }

            if ((originalPos == CardPosition.TRIO) && (dataManager.newDeck.Count > 0))
            {
                int cardValue = dataManager.newDeck[dataManager.newDeck.Count - 1];
                Transform instantiatedCard = dataManager.InstantiateCard(cardValue, true);
                shiftedTrio = dataManager.MoveCardIntoTrio(instantiatedCard, true, true, cardValue) == 1;
                dataManager.newDeck.RemoveAt(dataManager.newDeck.Count - 1);
            }
        }

        public override void Undo()
        {
            if (originalPos == CardPosition.TRIO)
            {
                if (shiftedTrio)
                {
                    dataManager.ShiftTrio();
                }
                dataManager.MoveCardIntoTrio(cardReference, false);
            }

            else if (originalPos == CardPosition.TOP_PILE)
            {
                dataManager.MoveCardIntoTopPile(cardReference, cardManager.group);
            }

            else
            {
                dataManager.MoveDescendingPileIntoBottomPile(cardReference, cardsMoved, originalPile);
                if (previousTurned)
                {
                    dataManager.SetPreviousCardVisibility(cardReference, false);
                }
            }
        }
    }
}

