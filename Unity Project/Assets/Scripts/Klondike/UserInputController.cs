using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klondike.Commands;

namespace Klondike
{
    public class UserInputController : MonoBehaviour
    {
        Transform selected;

        KlondikeController klondikeController;
        DataManager dataManager;

        public void Initialize()
        {
            klondikeController = GetComponent<KlondikeController>();
            dataManager = GetComponent<DataManager>();
        }

        public void Unselect()
        {
            CardManager selectionCardManager = selected.GetComponent<CardManager>();
            if (selectionCardManager.bottom)
            {
                dataManager.SetSelectionOfDescendingPile(selected, dataManager.DescendingPileSize(selected), false);
            }
            else
            {
                selectionCardManager.selected = false;
            }
            selected = null;
        }

        public void HandleReset()
        {
                klondikeController.RestartGame();
        }

        public void HandleUndo()
        {
                if (dataManager.commandHistory.Count > 0)
                {
                    if (selected)
                    {
                        Unselect();
                    }

                    Command undoneCommand = dataManager.commandHistory.Pop();
                    undoneCommand.Undo();
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
                        TopPileClickHandler(newSelection);
                    }
                    else if (hit.collider.CompareTag("Bottom"))
                    {
                        BottomPileClickHandler(newSelection);
                    }
                    else if (hit.collider.CompareTag("Deck"))
                    {
                        DeckClickHandler(newSelection);
                    }
                }
            }
        }

        void CardClickHandler(Transform newSelection)
        {
            void select()
            {
                CardManager newSelectionCardManager = newSelection.GetComponent<CardManager>();
                if (newSelectionCardManager.bottom)
                {
                    dataManager.SetSelectionOfDescendingPile(newSelection, dataManager.DescendingPileSize(newSelection), true);
                }
                else
                {
                    newSelectionCardManager.selected = true;
                }
                selected = newSelection;
            }
            bool eligibleTopPile(CardManager bullet, CardManager target) => (bullet.value == target.value + 1) && (bullet.group == target.group);
            bool eligibleBottomPile(CardManager bullet, CardManager target) => (bullet.value == target.value - 1) && (bullet.isRed ^ target.isRed);

            // If nothing is selected
            // Se nada está selecionado

            if (selected == null)
            {
                CardManager newSelectionCardManager = newSelection.GetComponent<CardManager>();
                
                // If the card is in the bottom piles
                // Se a carta está em nas pilhas de baixo

                if (newSelectionCardManager.bottom)
                {
                    int newSelectionSize = dataManager.DescendingPileSize(newSelection);

                    if ((newSelectionSize != -1) && (newSelectionCardManager.visible))
                    {
                        select();
                    }
                }
                
                // If the card is in the top trio
                // Se a carta está no trio visível

                else if (newSelectionCardManager.trio)
                {
                    if (!dataManager.IsCardInTrioBlocked(newSelection))
                    {
                        select();
                    }
                }

                // If the card is in the top piles
                // Se a carta está nas pilhas de cima

                else
                {
                    select();
                }
            }

            // If the new selection is the same as the current one
            // Se a nova seleção for a mesma da seleção atual

            else if (selected == newSelection)
            {
                Unselect();
            }

            // If the new selection is not the same as the current one
            // Se a nova seleção for diferente da seleção atual

            else if (selected != newSelection)
            {
                CardManager currentSelectionCardManager = selected.GetComponent<CardManager>();
                CardManager newSelectionCardManager = newSelection.GetComponent<CardManager>();

                // Corrigindo bug que permitia selecionar carta que não fosse o topo do monte
                if ((newSelectionCardManager.trio) && (!dataManager.IsCardInTrioBlocked(newSelection)))
                {
                    Unselect();
                    select();
                }

                else if (newSelectionCardManager.bottom)
                {
                    int newSelectionSize = dataManager.DescendingPileSize(newSelection);
                    if ((newSelectionSize != -1) && (newSelectionCardManager.visible))
                    {
                        if ((eligibleBottomPile(currentSelectionCardManager, newSelectionCardManager)) && newSelectionSize == 1)
                        {
                            MoveCardIntoBottomPile newCommand;

                            if (currentSelectionCardManager.trio)
                            {
                                newCommand = new MoveCardIntoBottomPile(selected, newSelectionCardManager.pile);
                            }
                            else if (currentSelectionCardManager.bottom)
                            {
                                newCommand = new MoveCardIntoBottomPile(selected, currentSelectionCardManager.pile, newSelectionCardManager.pile, dataManager.DescendingPileSize(selected));
                            }
                            else
                            {
                                newCommand = new MoveCardIntoBottomPile(selected, currentSelectionCardManager.group, newSelectionCardManager.pile);
                            }

                            dataManager.commandHistory.Push(newCommand);
                            newCommand.Execute();
                            Unselect();
                        }
                        else
                        {
                            Unselect();
                            select();
                        }
                    }
                }

                else if (newSelectionCardManager.top)
                {
                    int currentSize = dataManager.DescendingPileSize(selected);
                    if ((eligibleTopPile(currentSelectionCardManager, newSelectionCardManager)) && (currentSize == 1))
                    {
                        MoveCardIntoTopPile newCommand;

                        if (currentSelectionCardManager.trio)
                        {
                            newCommand = new MoveCardIntoTopPile(selected, newSelectionCardManager.group);
                        }
                        else if (currentSelectionCardManager.bottom)
                        {
                            newCommand = new MoveCardIntoTopPile(selected, currentSelectionCardManager.pile, newSelectionCardManager.group);
                        }
                        else
                        {
                            return;
                        }

                        dataManager.commandHistory.Push(newCommand);
                        newCommand.Execute();
                        Unselect();
                    }
                    else
                    {
                        Unselect();
                        select();
                    }
                }
            }
        }

        void TopPileClickHandler(Transform newSelection)
        {
            if (selected)
            {
                int targetPileNumber = int.Parse(newSelection.name[2].ToString());
                CardManager cardManager = selected.GetComponent<CardManager>();

                if ((cardManager.group == targetPileNumber) && (cardManager.value == 0))
                {
                    MoveCardIntoTopPile newCommand;

                    if (cardManager.trio)
                    {
                        newCommand = new MoveCardIntoTopPile(selected, targetPileNumber);
                    }

                    else
                    {
                        newCommand = new MoveCardIntoTopPile(selected, cardManager.pile, targetPileNumber);
                    }

                    dataManager.commandHistory.Push(newCommand);
                    newCommand.Execute();
                    Unselect();
                }
            }
        }

        void BottomPileClickHandler(Transform newSelection)
        {
            if (selected)
            {
                CardManager cardManager = selected.GetComponent<CardManager>();

                if (cardManager.value == 12)
                {
                    int targetPileNumber = int.Parse(newSelection.name[2].ToString());
                    MoveCardIntoBottomPile newCommand;

                    if (cardManager.bottom)
                    {
                        newCommand = new MoveCardIntoBottomPile(selected, cardManager.pile, targetPileNumber, dataManager.DescendingPileSize(selected));
                    }
                    else if (cardManager.top)
                    {
                        newCommand = new MoveCardIntoBottomPile(selected, cardManager.group, targetPileNumber);
                    }
                    else
                    {
                        newCommand = new MoveCardIntoBottomPile(selected, targetPileNumber);
                    }

                    dataManager.commandHistory.Push(newCommand);
                    newCommand.Execute();
                    Unselect();
                }
            }
        }

        void DeckClickHandler(Transform newSelection)
        {
            if (selected)
            {
                Unselect();
            }

            UseDeck newCommand = new UseDeck();
            dataManager.commandHistory.Push(newCommand);
            newCommand.Execute();
        }
    }
}
