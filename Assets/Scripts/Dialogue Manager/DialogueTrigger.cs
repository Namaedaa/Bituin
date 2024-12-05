using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    Dialogue DialogueObject;

    DialogueManager dialoguemanager;

    public UnityEvent[] dialogueEvents;

    private bool activated = false;
    void Start()
    {
        dialoguemanager = GameObject.Find("Dialogue Canvas").GetComponent<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated)
            return;
        if (collision.gameObject.tag == "Amelia" || collision.gameObject.tag == "Robot")
        {
            activated = true;
            runDialogue();
        }
    }

    private void runDialogue()
    {
        dialoguemanager.TakeDialogue(DialogueObject,dialogueEvents);
    }

    public void Reactivate()
    {
        activated = false;
    }
}
