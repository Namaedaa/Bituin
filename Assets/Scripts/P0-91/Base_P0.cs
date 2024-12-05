using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Base_P0 : MonoBehaviour
{
    protected GameController gamecontroller;
    
    //Movement and Rotation
    [SerializeField]
    protected float smoothInputVelocity, rotationSpeed;
    public float playerSpeed;

    internal static Vector2 playerDirection;
    protected Vector2 currentPosition;
    protected Vector2 currentVelocity;

    protected static SpriteRenderer po_sprite;
    protected Rigidbody2D po_rb;

    [SerializeField]
    Sprite[] headMovementSprites;


    // Respawning

    [Header("Death Settings")]

    [SerializeField]
    private float respawnTime = 2f;

    public static bool robotDied = false;

    protected Transform robotSpawnpoint;

    [Header("Materials")]

    [SerializeField]
    private Material deathMaterial;

    [SerializeField]
    private Material defaultMaterial;




    // Start is called before the first frame update
    protected void baseStart()
    {
        gamecontroller = GameObject.Find("Game Controller").GetComponent<GameController>();
        robotSpawnpoint = GameObject.Find("P0 Spawnpoint").GetComponent<Transform>();

        po_rb = this.GetComponent<Rigidbody2D>();
        currentPosition = this.GetComponent<Transform>().position;
        po_sprite = this.GetComponent<SpriteRenderer>();
    }

    protected void baseFixedUpdate()
    {
        MoveRobot();
    }

    internal void MoveRobot()
    {
        //for smooth rotation
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, playerDirection);
        po_rb.MoveRotation(Quaternion.SlerpUnclamped(this.transform.rotation, rotation, rotationSpeed * Time.deltaTime));

        //for actual smooth movement of player
        currentPosition = Vector2.SmoothDamp(po_rb.position, po_rb.position + playerDirection, ref currentVelocity, smoothInputVelocity / playerSpeed);
        po_rb.MovePosition(currentPosition);
    }

    //Changes Orientation
    protected void ChangeHeadOrientation(Vector2 worldPos)
    {
        float mouseDistanceTocCharPosition = worldPos.x - po_rb.position.x;

        /*Debug.Log("Distance to world pos " + mouseDistanceTocCharPosition);*/
        if (mouseDistanceTocCharPosition < 0)
        {

            if (mouseDistanceTocCharPosition <= -1)
            {
                po_sprite.sprite = headMovementSprites[0];
            }
            else if (mouseDistanceTocCharPosition > -1 && mouseDistanceTocCharPosition <= -0.5)
            {
                po_sprite.sprite = headMovementSprites[1];
            }
            else
            {
                po_sprite.sprite = headMovementSprites[2];
            }
        }
        else
        {

            if (mouseDistanceTocCharPosition >= 1)
            {
                po_sprite.sprite = headMovementSprites[4];
            }
            else if (mouseDistanceTocCharPosition < 1 && mouseDistanceTocCharPosition >= 0.5)
            {
                po_sprite.sprite = headMovementSprites[3];
            }
            else
            {
                po_sprite.sprite = headMovementSprites[2];
            }
        }
    }

    protected void baseOnTriggerEnter2D(Collider2D collision)
    {
        if (robotDied)
            return;

        if(collision.gameObject.tag == "RobotEnemy" || collision.gameObject.tag == "Projectile")
        {
            onDeath();
        }
    }

    protected void baseOnCollisionEnter2D(Collision2D collision)
    {
        if (robotDied)
            return;
        if (collision.gameObject.tag == "RobotEnemy" )
        {
            onDeath();
        }
    }

    private void onDeath()
    {
        robotDied = true;
        po_sprite.material = deathMaterial;

        Invoke("afterDeath", respawnTime);
    }

    private void afterDeath()
    {
        if (!robotDied)
            return;

        gamecontroller.respawnPlayer.Invoke();
        gamecontroller.resetLevel.Invoke();
    }

    public void respawn()
    {
        robotDied = false;
        transform.position = robotSpawnpoint.position;
        po_sprite.material = defaultMaterial;
    }




}
