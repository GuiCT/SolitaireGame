using System;
using System.Collections.Generic;
using Spider;
using UnityEngine;

namespace Spider.Commands
{
    class DistributeFromDeck : Command
    {
        static SpiderController spiderController;

        public DistributeFromDeck() : base()
        {
            if (spiderController == null)
            {
                spiderController = dataManager.gameObject.GetComponent<SpiderController>();
            }
        }

        public override void Execute()
        {
            spiderController.StartCoroutineByController(spiderController.UseDeck());
        }

        public override void Undo()
        {
            if (dataManager.EmptyDeck)
            {
                dataManager.deckButton.gameObject.SetActive(true);
            }

            for (int i = 9; i >= 0; i--)
            {
                Transform currentCardReference = dataManager.pilesObjects[i].GetChild(dataManager.piles[i].Count - 1);
                int currentCard = dataManager.piles[i][dataManager.piles[i].Count - 1];
                // Destroy the unity instance of the card
                UnityEngine.Object.Destroy(currentCardReference.gameObject);
                // Remove it from the int list
                dataManager.piles[i].RemoveAt(dataManager.piles[i].Count - 1);
                // Add it back to the deck
                dataManager.deck.Add(currentCard);
            }
        }
    }
}
