using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Klondike
{
    public class KlondikeController : MonoBehaviour
    {
        DataManager dataManager;
        UserInputController userInputController;
        bool gameIsPlaying;

        void Start()
        {
            gameIsPlaying = false;
            dataManager = GetComponent<DataManager>();
            dataManager.Initialize();
            userInputController = GetComponent<UserInputController>();
            userInputController.Initialize();
            // Distribuindo as cartas para as pilhas de baixo
            // Posicionando as cartas nas pilhas de baixo
            dataManager.GenerateDeck();
            dataManager.ShuffleDeck();
            StartCoroutine(DistributeDeckIntoPiles());
        }

        void Update()
        {
            if (gameIsPlaying)
            {
                

                userInputController.HandleClick();


            if (Input.GetKeyDown(KeyCode.End))
            {
                userInputController.HandleReset();
            }
     
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                userInputController.HandleUndo();
            }

            }
        }

        IEnumerator DistributeDeckIntoPiles()
        {
            gameIsPlaying = false;
            for (int i = 0; i < 7; i++)
            {
                int limit = i + 1;
                for (int j = 0; j < limit; j++)
                {
                    yield return new WaitForSeconds(0.03f);

                    Transform newCard = dataManager.InstantiateCard(dataManager.LastCardInDeck, j == limit-1);
                    dataManager.MoveCardIntoBottomPile(newCard, i, true, dataManager.LastCardInDeck);
                    dataManager.deck.RemoveAt(dataManager.deck.Count - 1);
                }
            }
            gameIsPlaying = true;
        }

        public void RestartGame()
        {
            MenuManager.instance.LoadGame(3);
        }
    }
}
