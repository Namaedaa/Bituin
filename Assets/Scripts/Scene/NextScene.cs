using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);  
    }
}
