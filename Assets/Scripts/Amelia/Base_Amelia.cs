using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;

public class Base_Amelia : MonoBehaviour
{
    protected GameController gamecontroller;

    protected Rigidbody2D amelia_rb;

    protected Animator amelia_animator;

    protected float x_movement;

    internal static float last_x_movement;

    protected Transform ameliaSpawnpoint;

    private SpriteRenderer ameliaRenderer;

    protected int obstacleLayerMask;

    protected int groundLayerMask;


    
    [Header("Movement Settings")]

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float maxSpeed;

    [Header("Death Settings")]

    [SerializeField]
    private float respawnTime = 2f;

    [Header("Amelia States")]

    [SerializeField]
    protected bool AmeliaJumped = false;

    [SerializeField]
    protected bool AmeliaRunning = false;

    [SerializeField]
    protected bool AmeliaGrounded = false;

    [SerializeField]
    public static bool AmeliaDied = false;


    [Header("Slope & Ground Settings")]

    [SerializeField]
    protected Transform GroundCheck;

    [SerializeField]
    public float GroundCheckRadius;

    [SerializeField]
    protected float slopeCheckDistance;

    [SerializeField]
    protected PhysicsMaterial2D fullFriction;

    protected Vector2 slopeNormalPerp;

    protected float slopeDownAngle;

    protected float slopeDownangleOld;

    protected bool isOnSlope;

    [Header("Materials")]
    [SerializeField]
    private Material deathMaterial;

    [SerializeField]
    private Material defaultMaterial;


    protected void baseStart()
    {
        amelia_rb = GetComponent<Rigidbody2D>();
        amelia_animator = transform.GetChild(0).GetComponent<Animator>();
        ameliaSpawnpoint = GameObject.Find("Amelia Spawnpoint").GetComponent<Transform>();
        ameliaRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        gamecontroller = GameObject.Find("Game Controller").GetComponent<GameController>();


        GroundCheck = transform.GetChild(1).GetComponent<Transform>();
        obstacleLayerMask = LayerMask.GetMask("Obstacles");
        groundLayerMask = LayerMask.GetMask("Ground");
    }

    protected void baseFixedUpdate()
    {
        AmeliaGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, obstacleLayerMask);
        bool groundchecker = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, groundLayerMask);

        AmeliaGrounded = groundchecker ? groundchecker : AmeliaGrounded;

        SlopeCheck();

        //Gravity
        //amelia_rb.velocity = EntityGravity.modifyEntityGravity(amelia_rb);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
    }

    protected void SlopeCheck()
    {
        SlopeCheckVertical(transform.position);
    }

    protected void SlopeCheckVertical(Vector2 checkPos)
    {
        //RaycastHit2D hit = Physics2D.BoxCast(checkPos, new Vector2(0.01f, 0.01f), 0, Vector2.down, slopeCheckDistance, obstacleLayerMask);
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, obstacleLayerMask);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownangleOld)
            {
                isOnSlope = true;
            }

            if (Mathf.Abs(slopeNormalPerp.x) == 1)
                isOnSlope = false;

            slopeDownangleOld = slopeDownAngle;

            //Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            //Debug.DrawRay(hit.point, hit.normal, Color.green);
        }

        if (isOnSlope && x_movement == 0.0f)
        {
            amelia_rb.sharedMaterial = fullFriction;
        }
        else
            amelia_rb.sharedMaterial = null;
    }

    protected void MoveAmelia()
    {
        float speedToUse = (AmeliaRunning && AmeliaGrounded ? maxSpeed : speed);

        if (AmeliaGrounded && !isOnSlope)
        {
            transform.position = Vector3.Slerp(transform.position, transform.position + new Vector3(x_movement * speedToUse, 0), speedToUse * Time.deltaTime);
        }
        else if (AmeliaGrounded && isOnSlope)
        {
            transform.position = Vector3.Slerp(transform.position, transform.position + new Vector3(speedToUse * slopeNormalPerp.x * -x_movement, speedToUse * slopeNormalPerp.y * -x_movement), speedToUse * Time.deltaTime);
        }
        else if (!AmeliaGrounded)
        {
            transform.position = Vector3.Slerp(transform.position, transform.position + new Vector3(x_movement * speedToUse, 0), speedToUse * Time.deltaTime);
        }
    }


    protected void AnimateMovement()
    {
        if (x_movement != 0)
            last_x_movement = x_movement;

        amelia_animator.SetFloat("Direction", x_movement);
        amelia_animator.SetFloat("IdleDirection", last_x_movement);
        amelia_animator.SetFloat("Speed", AmeliaRunning ? 3f : 1f);
        amelia_animator.SetBool("isMoving", x_movement != 0 ? true : false);
        amelia_animator.SetBool("isGrounded", AmeliaGrounded);
    }

    protected void baseOnTriggerEnter2D(Collider2D collision)
    {
        if (AmeliaDied)
            return;

        if (collision.gameObject.tag == "RobotEnemy" || collision.gameObject.tag == "Projectile")
        {
            onDeath();
        }
    }

    protected void baseOnCollisionEnter2D(Collision2D collision)
    {
        if (AmeliaDied)
            return;

        if (collision.gameObject.tag == "RobotEnemy")
        {
            onDeath();
        }
    }

    private void onDeath()
    {
        x_movement = 0;
        AnimateMovement();
        AmeliaDied = true;
        ameliaRenderer.material = deathMaterial;

        Invoke("afterDeath", respawnTime);
    }

    private void afterDeath()
    {
        if (!AmeliaDied)
            return;

        gamecontroller.respawnPlayer.Invoke();
        gamecontroller.resetLevel.Invoke();
    }

    public void respawn()
    {
        transform.position = ameliaSpawnpoint.position;
        AmeliaDied = false;
        ameliaRenderer.material = defaultMaterial;
    }
}
