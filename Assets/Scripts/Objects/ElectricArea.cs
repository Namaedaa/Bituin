using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ElectricArea : MonoBehaviour
{
   
    [Header("Zone Area")]
    [SerializeField] Transform originTransform;
    [SerializeField] float zoneRadius;
    [SerializeField] private List<Collider2D> zoneHitColliders;
    [SerializeField] LayerMask affectedLayer;
    private Vector3[] collidersVec3;
    [SerializeField] ContactFilter2D affectedFilter;

    [Header("Zone Duration and Activation")]    
    public bool isEnabled = true;
    [SerializeField] bool updateElectricZone;
    [SerializeField] private float currentTimer, maxTimer;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float hitBoxRadius;
    private GameObject projectile;
    private int electricBallsActive = 0;
    private SpriteRenderer electricZoneRend;
    float matOpacity = 0;
    [SerializeField] List<GameObject> electricBalls;

    [SerializeField] private float projectileSpeed;
    private Animator zoneMachineAnimator;


    // Start is called before the first frame update
    void Start()
    {
        originTransform = transform;
        zoneMachineAnimator = this.GetComponentInParent<Animator>();
        electricZoneRend = GetComponentInChildren<SpriteRenderer>();
        CheckElectricArea();
        currentTimer = maxTimer;

    }

    // Update is called once per frame
    void Update()
    {
        CheckElectricArea();
    }

    private void CheckElectricArea(){
        if (isEnabled)
        {
            zoneMachineAnimator.SetTrigger("Enable");
            CreateElectricArea();
            matOpacity = matOpacity < 1f ? matOpacity + Time.deltaTime : 1f;
        }
        else
        {
            zoneMachineAnimator.SetTrigger("Disable");
            if (electricBallsActive > 0)
            {
                for (int i = 0; i < electricBallsActive; i++)
                {
                    StopElectricBall(electricBalls[i]);
                    electricBalls.Remove(electricBalls[i]);
                    electricBallsActive--;
                }
            }
            matOpacity = matOpacity >= 0f ? matOpacity - Time.deltaTime : 0f;
        }
        electricZoneRend.color = new Color (1f,1f,1f,matOpacity);
    }
    public void ActivateElectricArea() 
    {
        isEnabled = true;   
    }

    public void DeactivateElectricArea()
    {
        isEnabled = false;
    }



    private void CreateElectricArea()
    {
        int resultsNum = Physics2D.OverlapCircle(originTransform.position, zoneRadius, affectedFilter, zoneHitColliders);
        
        collidersVec3 = new Vector3[resultsNum];
        if (resultsNum > 0)
        {
            for (int i = 0; i < resultsNum; i++)
            {
              
                if (zoneHitColliders[i] != null)
                {

                    collidersVec3[i] = zoneHitColliders[i].transform.position;
                    if (electricBallsActive < resultsNum)
                    {
                        CreateElectricBall();
                        electricBallsActive++;
                    }
                    if (electricBallsActive > 0)
                    {
                        /*currentTimer -= Time.deltaTime;*/
                        MoveElectricBall(electricBalls[i], collidersVec3[i]);
                    }
                     
                }
            
            }
        }
        else
        {
            if (electricBallsActive > 0)
            {
                for(int i=0; i < electricBallsActive;i++)
                {
                    StopElectricBall(electricBalls[i]);
                    electricBalls.Remove(electricBalls[i]);
                    electricBallsActive--;
                }
            }
        }
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(originTransform.position,zoneRadius);
    }

    private void CreateElectricBall()
    {
        projectile = Instantiate(projectilePrefab,this.transform.position,Quaternion.identity,this.transform);
        electricBalls.Add(projectile);

    }
     private void MoveElectricBall(GameObject electricBall,Vector3 targetLocation)
     {
        //Go to Targeted Location Here Joachim
        Animator projectileAnimator = electricBall.GetComponent<Animator>();
        projectileAnimator.SetTrigger("Activate");
        electricBall.transform.position = Vector2.MoveTowards(electricBall.transform.position, targetLocation, projectileSpeed * Time.deltaTime);
        /*RaycastHit2D hit2D = Physics2D.CircleCast(electricBall.transform.position, hitBoxRadius, targetLocation,Mathf.Infinity, affectedLayer);
        if(hit2D.collider != null)
        {
            Debug.Log("I kil player Real");
        }*/
     }
    private void StopElectricBall(GameObject electricBall)
    {
        Animator projectileAnimator = electricBall.GetComponent<Animator>();
        projectileAnimator.SetTrigger("Deactivate");
    }


}


