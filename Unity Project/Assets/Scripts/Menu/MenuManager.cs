using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    public Text percentage;
    public GameObject loadingScreen;
    
    private void Awake() 
    {
        instance = this;
        loadingScreen.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    public void LoadGame(int gameID) 
    {
        loadingScreen.gameObject.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1).name));
        scenesLoading.Add(SceneManager.LoadSceneAsync(gameID, LoadSceneMode.Additive));
        StartCoroutine(GetSceneLoadProgress());
    }


    float loadProgress;
    public IEnumerator GetSceneLoadProgress() 
    {

        for (int i = 0; i < scenesLoading.Count; i++) 
        {
            while (!scenesLoading[i].isDone) 
            {
                loadProgress = 0;
                foreach(AsyncOperation operation in scenesLoading) 
                {
                    loadProgress += operation.progress;
                }
                loadProgress = (loadProgress / scenesLoading.Count) * 100f;
                percentage.text = string.Format("{0}%", Mathf.RoundToInt(loadProgress));
                yield return null;
            }
        
        }
        loadingScreen.gameObject.SetActive(false);
    }
}
