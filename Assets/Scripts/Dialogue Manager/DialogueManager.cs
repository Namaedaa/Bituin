using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    private TextMeshProUGUI dialogueBox;

    public float textSpeedAnimation;

    private Transform AmeliaTextArea;
    private Transform RobotTextArea;


    private Queue<Dictionary<string, string>> dialogueQueue;
    private Queue<int> indexTrigger;
    private Queue<UnityEvent> eventQueue;

    private int dialogueIndex = -1;

    public bool eventTriggered = false;


    private bool stillTyping;

    public float nextDialogueCoolDown = 1f;

    private float currentDialogueCoolDown;

    private string currentSpeaker = "";

    // Dialogue Sounds

    private AudioSource ameliaSound;
    private AudioSource p0Sound;

    void Start()
    {
        AmeliaTextArea = GameObject.Find("Amelia Text").transform;
        RobotTextArea = GameObject.Find("Robot Text").transform;
        currentDialogueCoolDown = nextDialogueCoolDown;

        ameliaSound = GameObject.Find("Amelia Sound").GetComponent<AudioSource>();
        p0Sound = GameObject.Find("P0 Sound").GetComponent<AudioSource>();

        dialogueBox = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        dialogueQueue = new Queue<Dictionary<string, string>>();
    }

    private void Update()
    {
        if (!GameController.inCutscene)
            return;

        transform.position = getTextLocation(currentSpeaker);

        if (currentDialogueCoolDown <= nextDialogueCoolDown)
            currentDialogueCoolDown += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(currentDialogueCoolDown >= nextDialogueCoolDown)
            {
                nextDialogue();
            }
        }
    }

    public void TakeDialogue(Dialogue dialogue, UnityEvent[] sceneDialogueEvents)
    {
        string[] speakers = dialogue.speaker;
        string[] sentences = dialogue.sentence;
        indexTrigger = new Queue<int>(dialogue.eventIndex);
        eventQueue = new Queue<UnityEvent>(sceneDialogueEvents);
        dialogueIndex = -1;

        dialogueQueue = new Queue<Dictionary<string, string>>();
        for (int i = 0; i < speakers.Length; i++)
        {
            dialogueQueue.Enqueue(new Dictionary<string, string>()
            {
                {speakers[i],sentences[i] }
            });
        }

        GameController.inCutscene = true;
        nextDialogue();
    }


    public void nextDialogue()
    {
        if (eventTriggered)
            return;

        if(indexTrigger.Count > 0)
        {
            if(dialogueIndex >= indexTrigger.First())
            {
                indexTrigger.Dequeue();
                var eventToTrigger = eventQueue.Dequeue();
                eventToTrigger.Invoke();
                //eventTriggered = true;

                return;
            }
        }

        if(dialogueQueue.Count > 0 && !stillTyping)
        {
            var currentDialogue = dialogueQueue.Dequeue();

            var speaker = currentDialogue.Keys.First();
            var sentence = currentDialogue.Values.First();

            currentSpeaker = speaker;
            StartCoroutine(animateText(speaker,sentence));
            dialogueIndex++;
        } 
        else if(dialogueQueue.Count <= 0 && !stillTyping)
        {
            dialogueBox.text = "";
            GameController.inCutscene = false;
        }
    }

    IEnumerator animateText(string speaker, string sentence)
    {
        stillTyping = true;
        dialogueBox.text = "";
        for(int i = 0; i < sentence.Length; i++)
        {
            playDialogueSound(speaker);
            dialogueBox.text += sentence[i];
            yield return new WaitForSeconds(textSpeedAnimation);
        }

        ameliaSound.Stop();
        p0Sound.Stop();

        stillTyping = false;
        currentDialogueCoolDown = 0;
    }

    private void playDialogueSound(string speaker)
    {
        switch (speaker)
        {
            case "Amelia":
                ameliaSound.Play(); 
                break;
            case "Robot":
                p0Sound.Play(); 
                break;
            default:
                break;
        }
    }

    private Vector3 getTextLocation(string speaker)
    {
        switch(speaker)
        {
            case "Amelia":
                return AmeliaTextArea.position;
            case "Robot":
                return RobotTextArea.position;
            default:
                return Vector3.zero;
        }
    }

    public void eventDone()
    {
        eventTriggered = false;
    }

    public void TestEvent1()
    {
        Debug.Log("Event 1 Triggered");
    }

    public void TestEvent2()
    {
        Debug.Log("Event 2 Triggered");
    }
}
