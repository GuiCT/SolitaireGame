using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Spider
{
    public class SpiderController : MonoBehaviour
    {
        DataManager dataManager;
        UserInputController userInputController;
        bool gameIsPlaying;

        void Start()
        {
            gameIsPlaying = false;
            dataManager = GetComponent<DataManager>();
            dataManager.Initialize();
            dataManager.GenerateDeck();
            dataManager.ShuffleDeck();
            dataManager.ShuffleDeck();
            dataManager.ShuffleDeck();
            userInputController = GetComponent<UserInputController>();
            userInputController.Initialize();
            // Distribuindo as cartas para as pilhas de baixo
            // Posicionando as cartas nas pilhas de baixo

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
            for (int i = 0; i < 10; i++)
            {
                int limit = 5;
                if (i < 4)
                {
                    limit = 6;
                }

                for (int j = 0; j < limit; j++)
                {
                    yield return new WaitForSeconds(0.01f);

                    bool visible = (j == limit - 1);
                    Transform createdCard = dataManager.InstantiateCard(dataManager.LastCard, visible);

                    dataManager.MoveCardIntoPile(createdCard, i, true, dataManager.LastCard);
                    dataManager.deck.RemoveAt(dataManager.deck.Count - 1);
                }
            }
            gameIsPlaying = true;
        }

        public IEnumerator UseDeck()
        {
            gameIsPlaying = false;
            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(0.03f);

                Transform createdCard = dataManager.InstantiateCard(dataManager.LastCard, true);
                dataManager.MoveCardIntoPile(createdCard, i, true, dataManager.LastCard);
                dataManager.deck.RemoveAt(dataManager.deck.Count - 1);
            }

            if (dataManager.EmptyDeck)
            {
                dataManager.deckButton.gameObject.SetActive(false);
            }
            gameIsPlaying = true;
        }

        public void StartCoroutineByController(IEnumerator desiredCoroutine)
        {
            StartCoroutine(desiredCoroutine);
        }

        public void RestartGame()
        {
            MenuManager.instance.LoadGame(3);
        }
    }
}
