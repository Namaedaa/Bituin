using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpointTrigger : MonoBehaviour
{
    private Transform ameliaspawnpoint;
    private Transform robotspawnpoint;

    private Transform ameliaspawn;
    private Transform robotspawn;

    private void Start()
    {
        ameliaspawnpoint = GameObject.Find("Amelia Spawnpoint").transform;
        robotspawnpoint = GameObject.Find("P0 Spawnpoint").transform;


        ameliaspawn = transform.GetChild(0).transform;
        robotspawn = transform.GetChild(1).transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Amelia" || collision.gameObject.tag == "Robot")
        {
            ameliaspawnpoint.position = ameliaspawn.position;
            robotspawnpoint.position = robotspawn.position;
        }
    }
}
