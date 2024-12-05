using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        
    }

    public void NewGameButton()
    {
        Debug.Log("New Game");
        InGameMenu.inGame = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGameButton()
    {
        Debug.Log("Load Game");
        //Do load game
    }

    public void QuitButton()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
