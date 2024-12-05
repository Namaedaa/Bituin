using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class AI_P0 : Base_P0
{
    

    Seeker seeker;

    Path path;

    [Header("Pathfinding")]

    public Transform AmeliaTransform;

    int currentWayPoint = 0;

    float distanceToAmelia = 0f;

    [SerializeField]
    float nextWayPointDistance;

    [SerializeField]
    Vector3 pathfindingOffset;


    void Start()
    {
        base.baseStart();

        seeker = GetComponent<Seeker>();

        InvokeRepeating("FindPath",0,0.5f);
        gamecontroller.respawnPlayer.AddListener(FindPath);
    }

    // Update is called once per frame
    void Update()
    {
        if (robotDied || Base_Amelia.AmeliaDied)
        {
            path = null;
            return;
        }

        if (!CommandWheel.usingAmelia)
            path = null;
        else
        {
            distanceToAmelia = Vector2.Distance(transform.position, AmeliaTransform.position);
            FollowPath();
            ChangeHeadOrientation(AmeliaTransform.position);
        }
    }

    private void FixedUpdate()
    {
        if (robotDied || Base_Amelia.AmeliaDied)
            return;

        if(CommandWheel.usingAmelia)
            base.baseFixedUpdate();
    }

    void FindPath()
    {
        if (robotDied || Base_Amelia.AmeliaDied)
            return;

        if (CommandWheel.usingAmelia && distanceToAmelia >= 5f)
            FindPathToAmelia();
    }

    void FollowPath()
    {
        if (path == null)
        {
            return;
        }


        if (currentWayPoint >= path.vectorPath.Count)
        {
            playerDirection = Vector2.zero;
            return;
        }

        Vector3 currentPoint = path.vectorPath[currentWayPoint];

        float path_distance = Vector2.Distance(po_rb.position,currentPoint);

        playerDirection = (path.vectorPath[currentWayPoint] - new Vector3(po_rb.position.x,po_rb.position.y)).normalized;

        //playerDirection = new Vector2(temp_direction.x < 0 ? -1 : 1, temp_direction.y);

        if (path_distance < nextWayPointDistance)
            currentWayPoint++;
    }

    void FindPathToAmelia()
    {
        if(seeker.IsDone())
            seeker.StartPath(transform.position,AmeliaTransform.position+ (Player_Amelia.last_x_movement == 1 ? pathfindingOffset * new Vector2(-1,1): pathfindingOffset),onPathComplete);
    }

    void onPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CommandWheel.usingAmelia)
            base.baseOnTriggerEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(CommandWheel.usingAmelia) 
            base.baseOnCollisionEnter2D (collision);
    }
}
