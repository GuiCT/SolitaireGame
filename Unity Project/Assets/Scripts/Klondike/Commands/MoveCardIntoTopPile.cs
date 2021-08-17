using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Klondike.Commands
{
    public class MoveCardIntoTopPile : Command
    {
        CardManager cardManager;
        Transform cardReference;
        CardPosition originalPos;
        int originalPile;
        int targetPile;
        bool previousTurned;
        bool shiftedTrio;

        public MoveCardIntoTopPile(Transform cardReference, int targetPile) : base()
        {
            this.cardReference = cardReference;
            this.cardManager = cardReference.GetComponent<CardManager>();
            this.originalPos = CardPosition.TRIO;
            this.originalPile = -1;
            this.targetPile = targetPile;
        }

        public MoveCardIntoTopPile(Transform cardReference, int originalPile, int targetPile) : base()
        {
            this.cardReference = cardReference;
            this.cardManager = cardReference.GetComponent<CardManager>();
            this.originalPos = CardPosition.BOTTOM_PILE;
            this.originalPile = originalPile;
            this.targetPile = targetPile;
        }

        public override void Execute()
        {
            if (originalPos == CardPosition.BOTTOM_PILE)
            {
                previousTurned = dataManager.SetPreviousCardVisibility(cardReference, true);
            }

            dataManager.MoveCardIntoTopPile(cardReference, targetPile);

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

            else
            {
                cardManager.pile = originalPile;
                if (previousTurned)
                {
                    dataManager.SetPreviousCardVisibility(cardReference, false);
                }
            }
        }
    }
}