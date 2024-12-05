using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public UnityEvent resetLevel;
    public UnityEvent respawnPlayer;

    public static bool inCutscene = false;

    public bool isPlayerDead()
    {
        return Base_P0.robotDied && Base_Amelia.AmeliaDied;
    }
}
