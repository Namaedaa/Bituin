using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_P0 : Base_P0
{
    private Vector2 mousePos;
    private Vector2 worldPos;

    internal static float last_x_movement;

    private void Start()
    {
        base.baseStart();
    }
    void Update()
    {
        if (robotDied)
            return;

        if (!CommandWheel.usingAmelia)
            GetPlayerInputs();
    }

    private void FixedUpdate()
    {
        if (robotDied)
            return;

        if (!CommandWheel.usingAmelia)
            base.baseFixedUpdate();
    }

    void GetPlayerInputs()
    {
        //get axis numbers of directions
        float directionX = Input.GetAxisRaw("Horizontal");
        float directionY = Input.GetAxisRaw("Vertical");
        playerDirection = new Vector2(directionX, directionY).normalized;

        if (directionX != 0)
            last_x_movement = directionX;


        //change orientation relative to mouse and player position
        mousePos = Input.mousePosition;
        worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        ChangeHeadOrientation(worldPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!CommandWheel.usingAmelia)
            base.baseOnTriggerEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!CommandWheel.usingAmelia)
            base.baseOnCollisionEnter2D(collision);
    }
}
