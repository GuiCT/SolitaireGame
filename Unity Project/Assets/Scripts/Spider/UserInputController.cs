using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spider.Commands;

namespace Spider
{
    public class UserInputController : MonoBehaviour
    {
        Transform selected;
        SpiderController spiderController;
        DataManager dataManager;

        public void Initialize()
        {
            spiderController = GetComponent<SpiderController>();
            dataManager = GetComponent<DataManager>();
        }

        public void HandleReset()
        {
            if (Input.GetKeyDown(KeyCode.End))
            {
                spiderController.RestartGame();
            }
        }

        public void HandleClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit)
                {
                    Transform newSelection = hit.collider.transform;

                    if (hit.collider.CompareTag("Card"))
                    {
                        CardClickHandler(newSelection);
                    }
                    else if (hit.collider.CompareTag("Top"))
                    {
                        PileClickHandler(newSelection);
                    }
                    else if (hit.collider.CompareTag("Deck"))
                    {
                        DeckClickHandler(newSelection);
                    }
                }
            }
        }

        public void HandleUndo()
        {
                if (dataManager.commandHistory.Count > 0)
                {
                    
                    if (selected)
                    {
                       dataManager.SetSelectionOfDescendingPile(selected, dataManager.DescendingPileSize(selected), false);

                    }

                    Command undidCommand = dataManager.commandHistory.Pop();
                    undidCommand.Undo();

                    if (undidCommand.GetType() == typeof(FinishFullGroup))
                    {
                        Command undoCommandTwo = dataManager.commandHistory.Pop();
                        undoCommandTwo.Undo();
                    }
                }
            
        }

        void CardClickHandler(Transform newSelection)
        {
            // If nothing is selected => Select Card
            // Se nada está selecionado => Selecione a carta
            if (!selected)
            {
                int pileSize = dataManager.DescendingPileSize(newSelection);
                if ((pileSize != -1) && newSelection.GetComponent<CardManager>().visible)
                {
                    dataManager.SetSelectionOfDescendingPile(newSelection, pileSize, true);
                    selected = newSelection;
                }
            }

            // If there is a card selected, and it is not the same as the new selection
            // Se uma carta está selecionada, e não é a mesma da nova seleção
            else if (selected != newSelection)
            {
                int newSize = dataManager.DescendingPileSize(newSelection);
                if ((newSize != -1) && newSelection.GetComponent<CardManager>().visible)
                {
                    Transform currentParent = selected.parent;
                    int currentIndex = selected.GetSiblingIndex();
                    int currentSize = dataManager.DescendingPileSize(selected);
                    int currentPileNumber = int.Parse(selected.parent.name[1].ToString());
                    int targetPileNumber = int.Parse(newSelection.parent.name[1].ToString());

                    if (newSize > 1)
                    {
                        dataManager.SetSelectionOfDescendingPile(selected, currentSize, false);
                        dataManager.SetSelectionOfDescendingPile(newSelection, newSize, true);

                        selected = newSelection;
                    }
                    else if (newSize == 1)
                    {
                        if (selected.GetComponent<CardManager>().value == newSelection.GetComponent<CardManager>().value - 1)
                        {
                            MoveCardsIntoPile newCommand = new MoveCardsIntoPile(selected, currentPileNumber, targetPileNumber, currentSize);
                            dataManager.commandHistory.Push(newCommand);
                            newCommand.Execute();
                            if (dataManager.PileHasCompleteGroup(targetPileNumber))
                            {
                                FinishFullGroup finishFullGroup = new FinishFullGroup(targetPileNumber);
                                dataManager.commandHistory.Push(finishFullGroup);
                                finishFullGroup.Execute();
                            }
                            dataManager.SetSelectionOfDescendingPile(selected, currentSize, false);
                            selected = null;
                        }
                        else
                        {
                            dataManager.SetSelectionOfDescendingPile(selected, currentSize, false);
                            newSelection.GetComponent<CardManager>().selected = true;
                            selected = newSelection;
                        }
                    }
                }
            }
            else
            {
                int currentSize = dataManager.DescendingPileSize(selected);
                dataManager.SetSelectionOfDescendingPile(selected, currentSize, false);
                selected = null;
            }
        }

        void PileClickHandler(Transform newSelection)
        {
            if (selected)
            {
                if (selected.CompareTag("Card"))
                {
                    CardManager cardManager = selected.GetComponent<CardManager>();
                    int currentPile = cardManager.pile;
                    int targetPile = int.Parse(newSelection.name[1].ToString());
                    int currentSize = dataManager.DescendingPileSize(selected);
                    MoveCardsIntoPile newCommand = new MoveCardsIntoPile(selected, currentPile, targetPile, currentSize);
                    dataManager.commandHistory.Push(newCommand);
                    newCommand.Execute();
                    selected = null;
                }
            }
        }

        void DeckClickHandler(Transform newSelection)
        {
            if (selected)
            {
                selected.GetComponent<CardManager>().selected = false;
                selected = null;
            }

            if (!dataManager.EmptyDeck)
            {
                DistributeFromDeck newCommand = new DistributeFromDeck();
                dataManager.commandHistory.Push(newCommand);
                newCommand.Execute();
            }
        }
    }
}
