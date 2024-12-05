using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemPopUpManager : MonoBehaviour
{
    public GameObject itemPopUp;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    public void OpenItemPopUp(string name, string description)
    {
        itemPopUp.SetActive(!itemPopUp.activeSelf);
        itemNameText.text = name;
        itemDescriptionText.text = description;
    }
}
