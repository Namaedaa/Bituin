using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Amelia") || collision.gameObject.CompareTag("Robot"))
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
