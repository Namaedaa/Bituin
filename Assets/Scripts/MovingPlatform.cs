using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    List<Transform> PatrolPoints = new List<Transform>();

    int currentWayPoint = 0;

    bool reverseMovement = false;

    public float publicSpeed;

    private float privateSpeed;

    void Start()
    {
        Transform waypoints_parent = this.transform.parent.Find("Waypoints");
        for (int i = 0; i < waypoints_parent.childCount; i++)
        {
            PatrolPoints.Add(waypoints_parent.GetChild(i));
        }
        /*StartMovement();*/
        /*StopMovement();*/
    }

    // Update is called once per frame
    void Update()
    {
        PlatformMovement();
    }

    public void StartMovement()
    {
        privateSpeed = publicSpeed;
    }

    public void StopMovement()
    {
        privateSpeed = 0;
    }

    public void PlatformMovement()
    {
        float distance = Vector2.Distance(this.transform.position, PatrolPoints[currentWayPoint].position);
        transform.position = Vector2.MoveTowards(this.transform.position, PatrolPoints[currentWayPoint].position, privateSpeed * Time.deltaTime);
        if (distance < 0.5f && !reverseMovement)
        {
            if (currentWayPoint < PatrolPoints.Count - 1)
            {
                currentWayPoint++;
            }

            else
            {
                reverseMovement = true;
            }
        }
        if (distance < 0.5f && reverseMovement)
        {
            if (currentWayPoint > 0)
            {
                currentWayPoint--;
            }
            else
            {
                reverseMovement = false;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
{
        if (collision.gameObject.CompareTag("Amelia"))
        {
            if (collision.GetContact(0).normal.y == -1)
            {
                collision.transform.parent = this.transform;
            }
        }

    }


    private void OnCollisionExit2D(Collision2D collision)
    {          
        if (collision.gameObject.CompareTag("Amelia"))
            collision.transform.parent = null;
    }
}
