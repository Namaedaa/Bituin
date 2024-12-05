using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
