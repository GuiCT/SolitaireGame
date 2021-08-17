using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject sobreElementos, mainMenu;
    
    void Start()
    {
        hideAbout();
    }

    public void hideAbout() 
    {
        sobreElementos.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void showAbout() 
    {
        sobreElementos.SetActive(true);
        mainMenu.SetActive(false);
    }
}
