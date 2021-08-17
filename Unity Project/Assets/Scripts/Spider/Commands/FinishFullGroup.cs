using System;
using System.Collections.Generic;
using Spider;
using UnityEngine;

namespace Spider.Commands
{
    class FinishFullGroup : Command
    {
        static int counter;
        int pileNumber;
        int[] valueArray;
        bool previousTurned;

        public FinishFullGroup(int pileNumber) : base()
        {
            this.pileNumber = pileNumber;
            valueArray = new int[13];
        }

        public override void Execute()
        {
            previousTurned = dataManager.SetPreviousCardVisibility(dataManager.pilesObjects[pileNumber].GetChild(dataManager.pilesObjects[pileNumber].childCount - 13));

            for (int i = 0; i < 13; i++)
            {
                valueArray[i] = dataManager.piles[pileNumber][dataManager.piles[pileNumber].Count - 1];
                UnityEngine.Object.Destroy(dataManager.pilesObjects[pileNumber].GetChild(dataManager.piles[pileNumber].Count - 1).gameObject);
                dataManager.piles[pileNumber].RemoveAt(dataManager.piles[pileNumber].Count - 1);
            }

            dataManager.finishedGroups.GetChild(counter).gameObject.SetActive(true);
            counter++;
        }

        public override void Undo()
        {
            counter--;
            dataManager.finishedGroups.GetChild(counter).gameObject.SetActive(false);

            for (int i = 12; i >= 0; i--)
            {
                Transform instantiatedCard = dataManager.InstantiateCard(valueArray[i], true);
                dataManager.MoveCardIntoPile(instantiatedCard, pileNumber, true, valueArray[i]);
            }

            if (previousTurned)
            {
                dataManager.SetPreviousCardVisibility(dataManager.pilesObjects[pileNumber].GetChild(dataManager.pilesObjects[pileNumber].childCount - 13), false);
            }
        }
    }
}

