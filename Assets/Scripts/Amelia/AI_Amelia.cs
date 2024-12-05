using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEditor;
using UnityEngine;

public class AI_Amelia : Base_Amelia
{
    [Header("Pathfinding")]

    Transform AI_Target;

    [SerializeField]
    private Vector2 pathfindingOffset = new Vector2(-0.2f,1.5f);

    public Transform robotTransform;

    public TeamRAstar astar;

    public float outOfBoundsDistance = 35f;
    
    public float outofRangeFollowDistance = 30f;

    public float pathFindingRepeatDuration = 0.5f;

    public float FollowCooldown = 0.3f;

    public float nextWaypointDistance = 0.6f;

    int currentWayPoint = 0;

    List<Node> path;

    [Header("Pathfinding Constraints")]

    internal static bool InCutscence = false;

    void Start()
    {
        base.baseStart();

        AI_Target = GameObject.Find("AI Target").transform;
        InvokeRepeating("FindRobot", pathFindingRepeatDuration, FollowCooldown);
    }

    private void Update()
    {
        if (AmeliaDied || Base_P0.robotDied || CommandWheel.usingAmelia)
        {
            path = null;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (CommandWheel.current_command == CommandState.Stay && !InCutscence)
            {
                Vector3 click_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                click_position.z = Camera.main.nearClipPlane;

                AI_Target.position = click_position;

                astar.FindPath(amelia_rb.position, click_position, OnPathComplete);
            }
        }
    }

    void FixedUpdate()
    {
        if (AmeliaDied || Base_P0.robotDied)
            return;

        base.baseFixedUpdate();
        if (!CommandWheel.usingAmelia)
            FollowPath();
    }


    void FindRobot()
    {
        float xDistance = Mathf.Abs(robotTransform.position.x - (amelia_rb.position.x - 0.2f));
        float yDistance = Mathf.Min(1.5f,Mathf.Abs(robotTransform.position.y - (amelia_rb.position.y + 1.5f)));
        float distance = xDistance+yDistance;


        /*Debug.Log($"X DIstance: {xDistance}");
        Debug.Log($"Y DIstance: {yDistance}");*/

        if (CommandWheel.current_command == CommandState.Follow && distance > 3.5f ||
            CommandWheel.current_command == CommandState.Stay && (xDistance >= outOfBoundsDistance || yDistance >= outOfBoundsDistance))
        {
            if (Player_P0.last_x_movement > 0)
                astar.FindPath(amelia_rb.position, robotTransform.position - new Vector3(0.5f,0,0), OnPathComplete);
            else
                astar.FindPath(amelia_rb.position, robotTransform.position + new Vector3(0.7f, 0, 0), OnPathComplete);
        }
    }

    void OnPathComplete(List<Node> foundPath)
    {
        if (foundPath != null)
        {
            path = foundPath;
            currentWayPoint = 0;
        }
        else
            path = null;
    }

    void FollowPath()
    {
        if (path == null)
        {
            x_movement = 0;
            AnimateMovement();
            return;
        }

        if (currentWayPoint >= path.Count)
        {
            x_movement = 0;
            AnimateMovement();
            return;
        }

        float distance = Vector2.Distance(amelia_rb.position, path[currentWayPoint].position);

        Vector2 temp_vector = path[currentWayPoint].position;

        Debug.DrawLine(temp_vector, temp_vector + (Vector2.up * 10));

        x_movement = (temp_vector - amelia_rb.position).normalized.x > 0 ? 1f : -1f;

        //transform.position = Vector3.Slerp(amelia_rb.position, path[currentWayPoint].position, speed * Time.deltaTime);
        MoveAmelia();
        AnimateMovement();


        if (distance < nextWaypointDistance)
            currentWayPoint++;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!CommandWheel.usingAmelia)
            baseOnTriggerEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!CommandWheel.usingAmelia)
            baseOnCollisionEnter2D(collision);
    }
}
