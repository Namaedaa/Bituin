using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class InteractionMechanic : MonoBehaviour
{

    public InteractableObjects interactableObjectsValues;

    #region
    [Header("Timers and Triggers")]
    [SerializeField]private float currentInteractionTime, interactionTimeStep;
    [SerializeField] private float completionRate;
    [SerializeField] private GameObject[] indicator;
    [SerializeField] private GameObject timerBar;
    [SerializeField] private Image fillupBar;
    [SerializeField] private bool isInteracted = false, isCompleted = false, isInteractable, isPressed = false;
    [SerializeField] private AudioSource timerSound;
    private AstarPath pathGraph;
    #endregion


    #region
    [Header("Interaction")]
    [SerializeField] private UnityEvent interactionStarting, interactionPause, interactionComplete, interactionReset;
    [SerializeField] private TextMeshProUGUI interactText;
    #endregion



    PlayerData PlayerData;
    int LevelState;


    // Start is called before the first frame update
    internal void Start()
    {
        currentInteractionTime = interactableObjectsValues.maxInteractionTime;
        timerSound = GameObject.Find("Timer Sound").GetComponent<AudioSource>();
        interactText = this.GetComponentInChildren<TextMeshProUGUI>();
        pathGraph = GameObject.Find("A*").GetComponent<AstarPath>();
        /* LevelState = LoadSaveSystem.player_data == null ? 0 : LoadSaveSystem.player_data.level_state;*/
    }

    // Update is called once per frame
    void Update()
    {
        //interact text
        InteractingText();

      /* if(LevelState > interactableObjectsValues.SaveState)
       {
            isCompleted = true;
       }*/
        //get key
        if (!isCompleted)
        {

            ShowIndicators(true);
            if (isInteractable)
            {
                if (CommandWheel.usingAmelia == interactableObjectsValues.forAmelia || !CommandWheel.usingAmelia == interactableObjectsValues.forRobot)
                {
                    if(Input.GetKeyDown(KeyCode.F))
                        timerSound.Play();

                    if (Input.GetKey(KeyCode.F))
                    {
                        interactionStarting.Invoke();
                    }
                    else
                    {
                        interactionPause.Invoke();
                    }
                }
                else
                {
                    isInteractable = false;
                }
            }

        }
        if ( isInteracted && interactableObjectsValues.isNormalTimer)
        {
            ShowTimer(true);
            NormalTimerMechanic();
        }
        else if (interactableObjectsValues.isPressThenTimer && isPressed)
        {
            ShowTimer(true);
            PushTimerMechanic();
            
        }

    }

    private void InteractingText()
    {

        if (isInteractable)
        {
            if (interactableObjectsValues.isNormalTimer)
            {
                interactText.text = "Hold F to interact";

            }
            else if (interactableObjectsValues.isPressThenTimer)
            {
                interactText.text = "Press F to interact";
            }
        }
        else
        {
            interactText.text = "";
        }

    }

    private void ShowIndicators(bool show)
    {
        if (show)
        {
            if (interactableObjectsValues.forAmelia)
            {
                indicator[0].SetActive(true);
                indicator[1].SetActive(false);
            }
            if (interactableObjectsValues.forRobot)
            {
                indicator[0].SetActive(false);
                indicator[1].SetActive(true);
            }

        }
        else
        {
            indicator[0].SetActive(false);
            indicator[1].SetActive(false);
        }
    }
    private void ShowTimer(bool show)
    {
        if (show)
        {
            timerBar.SetActive(true);
            //Debug.Log(100/completionRate);
            fillupBar.fillAmount = completionRate/100; 
        }
        else
        {
            timerBar.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isCompleted)
        {
            if ((collision.gameObject.CompareTag("Amelia") && interactableObjectsValues.forAmelia) || (collision.gameObject.CompareTag("Robot") && interactableObjectsValues.forRobot))
            {
                if (!CommandWheel.usingAmelia && interactableObjectsValues.forRobot)
                {
                  
                    isInteractable = true;

                }
                else if (CommandWheel.usingAmelia && interactableObjectsValues.forAmelia)
                {
                   
                    isInteractable = true;

                }

                if (!CommandWheel.usingAmelia && interactableObjectsValues.forAmelia)
                {
                    
                    isInteractable = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if ((collision.gameObject.CompareTag("Amelia") && interactableObjectsValues.forAmelia) ||(collision.gameObject.CompareTag("Robot") && interactableObjectsValues.forRobot))
        {
            StopInteracting();
            isInteractable = false;
            
        }

    }

    public void StartInteracting()
    {
        isInteracted = true;
        isPressed = true;
    }
    public void StopInteracting()
    {
        isInteracted = false;
    }

    public void ResetInteraction()
    {
        if (interactableObjectsValues.isReusable)
        {
            interactionReset.Invoke();
            ShowIndicators(true);
            ShowTimer(true);
            currentInteractionTime = interactableObjectsValues.maxInteractionTime;
            //Debug.Log(currentInteractionTime);    
            isPressed = false;
            isInteracted = false;
            isInteractable = false;
            isCompleted = false;
            //Debug.Log("Resetting...");
        }
    }

    public void CompleteInteraction()
    {
        interactionComplete.Invoke();
        ShowIndicators(false);
        ShowTimer(false);
        currentInteractionTime = interactableObjectsValues.maxInteractionTime;
        //Debug.Log(currentInteractionTime);
        isPressed = false;
        isInteracted = false;
        isInteractable = false;
        isCompleted = true;
        //Debug.Log("Completed");

        Invoke("scanGraph", 1f);
    }

    private void scanGraph()
    {
        pathGraph.Scan();
    }
    //Current time lessens when interacted
    private void NormalTimerMechanic()
    {
        currentInteractionTime -= interactionTimeStep * Time.deltaTime;
        completionRate = (currentInteractionTime / interactableObjectsValues.maxInteractionTime) * 100;
    /*    //Debug.Log("Completion Rate :" + completionRate);*/
        completionRate = Mathf.Clamp(completionRate, 0, 100);

        if (completionRate <= 0 && !isCompleted)
        {
            completionRate = 0;
            CompleteInteraction();
            ResetInteraction();
        }
        
    }



    //Current time lessens automatically after first interaction, then resets
    private void PushTimerMechanic()
    {
     
        if (isPressed)
        {
            currentInteractionTime -= interactionTimeStep * Time.deltaTime;
            completionRate = (currentInteractionTime / interactableObjectsValues.maxInteractionTime) * 100;
            /*  //Debug.Log("Completion Rate :" + completionRate);*/
            completionRate = Mathf.Clamp(completionRate, 0, 100);
            if (completionRate <= 0)
            {
                completionRate = 0;
                CompleteInteraction();
                ResetInteraction();
            }
            else
            {
                interactionStarting.Invoke();
            }
        }
        

    }


}
