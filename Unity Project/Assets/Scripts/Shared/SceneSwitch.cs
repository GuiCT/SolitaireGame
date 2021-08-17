using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneSwitch : MonoBehaviour
{
    public void loadScene(int gameID) 
    {
        MenuManager.instance.LoadGame(gameID);
    }
}
