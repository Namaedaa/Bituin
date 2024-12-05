using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Collectible : MonoBehaviour
{
    public Item item;

    ItemPopUpManager itemPopUpManager; 

    [SerializeField]
    private TMP_Text pickUpText;

    private bool pickUpAllowed;

    private void Start()
    {
        itemPopUpManager = FindObjectOfType<ItemPopUpManager>();
        pickUpText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (pickUpAllowed && Input.GetKeyDown(KeyCode.F))
            PickUp();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("PlayerTest"))
        {
            pickUpText.gameObject.SetActive(true);
            pickUpAllowed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name.Equals("Player"))
        {
            pickUpText.gameObject.SetActive(false);
            pickUpAllowed = false;
        }
    }

    private void PickUp()
    {
        itemPopUpManager.OpenItemPopUp(item.name, item.description);
        Inventory.instance.Add(item);
        Destroy(gameObject);
    }
}
