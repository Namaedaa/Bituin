using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    List<Transform> PatrolPoints = new List<Transform>();
    bool reverseMovement = false;

    public float speed;

    private float distance;

    private Rigidbody2D rb;

    int currentWayPoint = 0;

    public float rotationSpeed;
    private Quaternion adjustedDirection;

    VisionCone enemyVisionCone;

    [Header("Pathfinding")]

    int currentPathFindingWayPoint = 0;

    public float activateDistance = 50f;
    public float nextWaypointDistance = 0.1f;

    Seeker seeker;

    Path path;

    int obstacleMask = 0;

    [Header("AI Movement")]

    [SerializeField]
    private float playerFoundTempcountdown;

    [SerializeField]
    private float playerFoundcountdown;

    [SerializeField]
    private float findPlayerCountdown;

    [SerializeField]
    private float findPlayerTempCountdown;

    private bool playerFound = false;

    private Transform playerDetect;

    private Vector3 lastSeenPlayer;

    private Vector3 lastPosition;

    private Vector3 targetVector;

    private Tentacle[] tentacle;

    private bool isDead = false;

    private SpriteRenderer spriteRenderer;

    private CapsuleCollider2D enemyCollider;

    [SerializeField]
    private Material deathMaterial;

    [SerializeField]
    private Material defaultMaterial;

    public void Start()
    {
        // Pathfinding initialization
        seeker = GetComponent<Seeker>();
        obstacleMask = LayerMask.GetMask("Obstacles");
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<CapsuleCollider2D>();

        enemyVisionCone = GetComponentInChildren<VisionCone>();

        // Movement
        playerFoundTempcountdown = playerFoundcountdown;
        findPlayerTempCountdown = findPlayerCountdown;

        //
         tentacle = this.GetComponentsInChildren<Tentacle>();

        Transform waypoints_parent = this.transform.parent.Find("Waypoints");
        for (int i = 0; i < waypoints_parent.childCount; i++)
        {
            PatrolPoints.Add(waypoints_parent.GetChild(i));
        }
        this.transform.position = PatrolPoints[0].position;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(isDead) return;

        playerDetect = enemyVisionCone.EnemyVisionCheck();

        if (playerDetect != null)
        {
            playerFoundTempcountdown = playerFoundcountdown;
            lastSeenPlayer = playerDetect.position;

            playerFound = true;
        }
    }
    private void FixedUpdate()
    {
        if (isDead) return;

        if (playerFound)
        {
            calculate_path_to_target();
            MoveToPlayer();
        }
        else
        {
            if(lastSeenPlayer == Vector3.zero)
            {
                EnemyPatrol();
            }
            else
            {
                calculate_path_to_target();
                MoveToLastSeen();
            }
        }
        
    }
    

    public void EnemyMove(Vector3 TargetLocation)
    {
        /*distance = Vector2.Distance(transform.position, objectToBeFound);
        Vector2 direction = (objectToBeFound - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion adjustedDirection = Quaternion.Euler(Vector3.forward * angle);
        transform.position = Vector2.MoveTowards(this.transform.position, objectToBeFound, speed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, adjustedDirection, rotationSpeed * Time.deltaTime);*/

        targetVector = TargetLocation;

        if (path == null)
            return;

        if (currentPathFindingWayPoint >= path.vectorPath.Count)
            return;


        Vector3 currentWayPoint = path.vectorPath[currentPathFindingWayPoint];

        float distance = Vector2.Distance(transform.position, currentWayPoint);

        Vector2 direction = Vector2.zero;

        if (playerFound)
            direction = (lastSeenPlayer - transform.position).normalized;
        else
            direction = (currentWayPoint - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion adjustedDirection = Quaternion.Euler(Vector3.forward * angle);
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, adjustedDirection, rotationSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime);

        if (distance < nextWaypointDistance)
            currentPathFindingWayPoint++;
    }

    void MoveToPlayer()
    {
        playerFoundTempcountdown -= Time.deltaTime;
        if (playerFoundTempcountdown <= 0)
        {
            playerFound = false;
        }
        else
            EnemyMove(lastSeenPlayer);
    }

    void MoveToLastSeen()
    {
        EnemyMove(lastSeenPlayer);
        findPlayerTempCountdown -= Time.deltaTime;
        if (findPlayerTempCountdown <= 0f)
        {
            findPlayerTempCountdown = findPlayerCountdown;
            lastSeenPlayer = Vector3.zero;
            EnemyReturnToPosition();
        }
    }

    void calculate_path_to_target()
    {
        if(seeker.IsDone())
        {
            seeker.StartPath(transform.position,targetVector, onPathComplete);
        }
    }

    void onPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    public void EnemyReturnToPosition()
    {
        transform.position = PatrolPoints[currentWayPoint].position;
        rb.velocity = Vector2.zero;
        foreach (Tentacle appendage in tentacle)
        {
            appendage.ResetPos();
        }
        //transform.rotation = Quaternion.identity;
    }

    public void EnemyPatrol()
    {

        if (playerFound)
            return;

        float distance = Vector2.Distance(this.transform.position, PatrolPoints[currentWayPoint].position);
        transform.position = Vector2.MoveTowards(this.transform.position, PatrolPoints[currentWayPoint].position, speed * Time.deltaTime);
        if (distance < 0.5f && !reverseMovement)
        {
            if (currentWayPoint < PatrolPoints.Count - 1)
            {
                currentWayPoint++;
                flip();
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
                flip();

            }
            else
            {
                reverseMovement = false;
            }
        }
        transform.rotation = Quaternion.RotateTowards(this.transform.rotation, adjustedDirection, rotationSpeed * Time.deltaTime);
    }

    private void flip()
    {
            Vector2 direction = (PatrolPoints[currentWayPoint].position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            adjustedDirection = Quaternion.Euler(Vector3.forward * angle); 
      
          
    }

    private void respawnEnemy()
    {
        isDead = false;
        spriteRenderer.material = defaultMaterial;
        enemyCollider.enabled = true;
        this.transform.position = PatrolPoints[0].position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.velocity = Vector2.zero;
        if (collision.gameObject.tag == "Player")
        {
           //Do Something to game over or reset level
        }

        if(collision.gameObject.tag == "Projectile")
        {
            isDead = true;
            spriteRenderer.material = deathMaterial;
            enemyCollider.enabled = false;
            Invoke("respawnEnemy", 5f);
        }
        
    }
}
