using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBackToMainMenu : MonoBehaviour
{

    public int sceneIndex;

    private void Start()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
