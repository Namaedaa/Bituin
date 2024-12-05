using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Interactable Object", menuName = "InteractableObject")]
public class InteractableObjects : ScriptableObject
{

    #region
    [Header("Interacted Object Param")]
    public bool forRobot, forAmelia;
    public bool isNormalTimer, isPressThenTimer,isReusable, isRelic;
    public int SaveState;
    #endregion

    #region
    [Header("Timers and Triggers")]
    public float maxInteractionTime;
    #endregion


   


}
