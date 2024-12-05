using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public static bool inGame = false;

    private bool gamePaused = false;

    private GameObject black_bg;
    private GameObject pause_canvas;
    private GameObject option_canvas;
    void Start()
    {
        black_bg = transform.GetChild(0).gameObject;
        option_canvas = transform.GetChild(1).gameObject;
        pause_canvas = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inGame)
            return;


        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
        {
            gamePaused = !gamePaused;
            triggerPauseCanvas();
        }
    }

    void triggerPauseCanvas()
    {
        if(gamePaused)
        {
            Time.timeScale = 0f;
            black_bg.SetActive(true);
            pause_canvas.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            black_bg.SetActive(false);
            option_canvas.SetActive(false);
            pause_canvas.SetActive(false);
        }
    }

    public void backToMain()
    {
        SceneManager.LoadScene(0);
        gamePaused = false;
        inGame = false;
        triggerPauseCanvas();
    }

    public void backToGame()
    {
        gamePaused = false;
        triggerPauseCanvas();
    }
}
