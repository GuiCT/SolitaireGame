using System;
using UnityEngine;
using Spider;

namespace Spider.Commands
{
    class MoveCardsIntoPile : Command
    {
        Transform cardReference;
        int originalPile;
        int targetPile;
        int cardsMoved;
        bool previousTurned;

        public MoveCardsIntoPile(Transform cardReference, int originalPile, int targetPile, int cardsMoved) : base()
        {
            this.cardReference = cardReference;
            this.originalPile = originalPile;
            this.targetPile = targetPile;
            this.cardsMoved = cardsMoved;
        }

        public override void Execute()
        {
            previousTurned = dataManager.SetPreviousCardVisibility(cardReference);
            dataManager.MoveDescendingPile(cardReference, cardsMoved, targetPile);
            dataManager.SetSelectionOfDescendingPile(cardReference, cardsMoved, false);
        }

        public override void Undo()
        {
            if (cardReference == null)
            {
                int targetPileSize = dataManager.pilesObjects[targetPile].childCount;
                // Card was destroyed, find it again
                cardReference = dataManager.pilesObjects[targetPile].GetChild(targetPileSize - cardsMoved);
            }

            dataManager.MoveDescendingPile(cardReference, cardsMoved, originalPile);
            if (previousTurned)
            {
                dataManager.SetPreviousCardVisibility(cardReference, false);
            }
        }
    }
}
