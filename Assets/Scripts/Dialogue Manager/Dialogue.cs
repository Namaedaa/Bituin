using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "DialogueObject")]
public class Dialogue : ScriptableObject
{
    public string[] speaker;
    [TextArea(1,5)]
    public string[] sentence;
    public int[] eventIndex; 
}
