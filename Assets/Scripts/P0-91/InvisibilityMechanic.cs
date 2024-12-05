using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InvisibilityMechanic : MonoBehaviour
{

    public Material[] HolographicMats;
    public Material[] NormalMats;
    private float spriteAlpha = 1f;
    //For Robot
    private GameObject[] FLeg, BLeg;
    private Renderer spriteRenderer;
    public static bool isVisible = true;
    private bool isPressed = false;
    [SerializeField]
    private float cooldownTimer, tempCooldownTimer;
    private SpriteRenderer sprite;

    [SerializeField]
    private bool invisObtained = false;


    // Start is called before the first frame update
    void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
      
        FLeg = GameObject.FindGameObjectsWithTag("Fleg");
        BLeg = GameObject.FindGameObjectsWithTag("Bleg");

        spriteRenderer = this.GetComponent<Renderer>();

        //transparency & invisibility
        tempCooldownTimer = cooldownTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (CommandWheel.usingAmelia)
        {
            isVisible = InvisibileMode(false);
        }
        else if (invisObtained && !CommandWheel.usingAmelia)
        {
            //Invis when button pressed and cooldown
            if (Input.GetButtonDown("Jump"))
            {
                isVisible = InvisibileMode(isVisible);
                isPressed = true;
            }
        }

        if (isVisible)
        {
            spriteAlpha = spriteAlpha < 1f ? spriteAlpha += Time.deltaTime : spriteAlpha = 1f;
            sprite.color = new Color(1f, 1f, 1f, spriteAlpha);
            tempCooldownTimer = tempCooldownTimer < cooldownTimer ? tempCooldownTimer += Time.deltaTime : tempCooldownTimer = cooldownTimer;
        }
        else
        {
            spriteAlpha = spriteAlpha >= 0.65f ? spriteAlpha -= Time.deltaTime : spriteAlpha = 0.65f;
            sprite.color = new Color(1f, 1f, 1f, spriteAlpha);
            if (isPressed)
            {
                tempCooldownTimer -= Time.deltaTime;
                if (tempCooldownTimer <= 0f)
                {
                    isVisible = InvisibileMode(false);
                    isPressed = false;
                }
            }
        }
    }

    //Changes Material to Invisibility
    public bool InvisibileMode(bool isVisible)
    {
        if (!isVisible)
        {
            spriteRenderer.material = NormalMats[0];
            this.gameObject.layer = 10;
            foreach (GameObject leg in FLeg)
            {
                leg.GetComponent<Renderer>().material = NormalMats[1];
            }
            foreach (GameObject leg in BLeg)
            {
                leg.GetComponent<Renderer>().material = NormalMats[2];
            }
            return true;
        }
        else
        {
            spriteRenderer.material = HolographicMats[0];
            this.gameObject.layer = 1;
            foreach (GameObject leg in FLeg)
            {
                leg.GetComponent<Renderer>().material = HolographicMats[1];
            }
            foreach (GameObject leg in BLeg)
            {
                leg.GetComponent<Renderer>().material = HolographicMats[2];
            }
            return false;
        }
        
            
    }
}
