using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Amelia : Base_Amelia
{
    void Start()
    {
        base.baseStart();
    }

    void Update()
    {
        if (AmeliaDied)
            return;

        if(CommandWheel.usingAmelia)
            GetPlayerInputs();
    }

    void FixedUpdate()
    {
        if (AmeliaDied)
            return;


        base.baseFixedUpdate();
        if(CommandWheel.usingAmelia)
        {
            MoveAmelia();

            if(AmeliaJumped)
            {
                amelia_rb.velocity = Vector2.up * speed * 5f;

                AmeliaGrounded = false;
                AmeliaJumped = false;
            }
        }
    }

    void GetPlayerInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space) && AmeliaGrounded)
        {
            //Debug.Log("Jumping now");
            AmeliaJumped = true;
        }

        if (Input.GetKey(KeyCode.LeftShift) && AmeliaGrounded)
        {
            AmeliaRunning = true;
        }
        else if (AmeliaGrounded)
        {
            AmeliaRunning = false;
        }
        // Debug.Log("Is amelia Running: " +AmeliaRunning);
        x_movement = Input.GetAxis("Horizontal");
        AnimateMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CommandWheel.usingAmelia)
            baseOnTriggerEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CommandWheel.usingAmelia)
            baseOnCollisionEnter2D(collision);
    }

}
