using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CommandState
{
    Stay, Follow, Switch
}

public class CommandWheel : MonoBehaviour
{
    internal static CommandState current_command = CommandState.Follow;

    int wheel_layer;

    Vector3 current_mouse_pos;

    [SerializeField]
    GameObject Command_Canvas;

    internal static CommandState highlighted_command;

    public Transform robotTransform;
    public Transform ameliaTransform;

    public SpriteRenderer ameliaRenderer;
    public SpriteRenderer p0renderer;


    public CinemachineVirtualCamera cameraFollow;

    internal static bool usingAmelia = false;

    Vector3 onclick_location;
    void Start()
    {
        Command_Canvas.SetActive(false);
        wheel_layer = LayerMask.GetMask("Command Wheel Layer");

        robotTransform = GameObject.Find("Robot").transform;
        ameliaTransform = GameObject.Find("Amelia").transform;

        ameliaRenderer = ameliaTransform.GetComponent<SpriteRenderer>();
        p0renderer = robotTransform.GetComponent<SpriteRenderer>();

        p0renderer = robotTransform.GetComponent<SpriteRenderer>();
        ameliaRenderer = ameliaTransform.GetComponent<SpriteRenderer>();
        
        cameraFollow.Follow = robotTransform;
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            onclick_location = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.main.nearClipPlane));
            Command_Canvas.transform.position = onclick_location;
            Command_Canvas.SetActive(true);
        }
        if (Input.GetMouseButton(1))
        {
            current_mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            current_mouse_pos.z = Camera.main.nearClipPlane;
            Vector3 direction = current_mouse_pos - onclick_location;

            RaycastHit2D hit = Physics2D.Raycast(onclick_location, direction, Mathf.Infinity, wheel_layer);

            if(hit)
            {
                set_command(hit.transform.name);
            }
        }
        if(Input.GetMouseButtonUp(1))
        {
            current_command = highlighted_command;
            //Debug.Log("Chosen Command"+ current_command);
            Command_Canvas.SetActive(false);

            if(current_command == CommandState.Switch)
            {
                Base_P0.playerDirection = Vector2.zero;
                usingAmelia = !usingAmelia;
            }
        }

        if(usingAmelia)
        {
            cameraFollow.Follow = ameliaTransform;
            ameliaRenderer.sortingOrder = 3;
            p0renderer.sortingOrder = 2;
        }
        else
        {
            cameraFollow.Follow = robotTransform;
            ameliaRenderer.sortingOrder = -5;
            p0renderer.sortingOrder = 3;
        }
    }

    void set_command(string command_name)
    {
        if (command_name == "Follow")
            highlighted_command = CommandState.Follow;
        else if (command_name == "Stay")
            highlighted_command = CommandState.Stay;
        else if (command_name == "Switch")
            highlighted_command = CommandState.Switch;
    }
}
