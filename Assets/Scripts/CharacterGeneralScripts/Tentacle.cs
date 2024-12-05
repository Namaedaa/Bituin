using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public LineRenderer lineRend;
    public int length;
    public Vector3[] segmentPoses;
    private Vector3[] segmentV;
    [SerializeField]
    private bool isSegmented = false;

    public Transform targetDir;
    public float targetDist;
    public float smoothSpeed;
    [SerializeField]
    private float wiggleSpeed, wiggleMagnitude;


    [SerializeField]
    private float stationaryTolerance,wiggleTimer;
    private Vector3 lastPos;
    private bool doWiggle = false;
    public Transform wiggleDir;

    public Transform tailEnd;
    public Transform[] bodyParts;

    // Start is called before the first frame update
    void Start()
    {
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
        ResetPos();
        InvokeRepeating("CheckLastPos", 0f, wiggleTimer);

    }

     void Update()
    {
        //For Wiggle when moving and not moving
        float traveled = (lastPos - this.transform.position).magnitude;
        if (this.transform.position != lastPos && traveled >= stationaryTolerance)
        {
            doWiggle = true;
        }
        else
        {
            doWiggle = false;
        }


    }
    //For Wiggle when moving and not moving
    void CheckLastPos()
    {
        lastPos = this.transform.position;
    }

   public void ResetPos()
    {
        segmentPoses[0] = targetDir.position;
        for(int i = 1; i < length; i++)
        {
            segmentPoses[i] = segmentPoses[i - 1] + targetDir.right * targetDist;
        }
        lineRend.SetPositions(segmentPoses);
    }

    void NormalSegment()
    {
        segmentPoses[0] = targetDir.position;
        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + targetDir.right * targetDist, ref segmentV[i], smoothSpeed);
        }
        lineRend.SetPositions(segmentPoses);
    }

    void SegmentedBody()
    {
        segmentPoses[0] = targetDir.position;
        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + targetDir.right * targetDist, ref segmentV[i], smoothSpeed);
            bodyParts[i - 1].transform.position = segmentPoses[i];
        }
        lineRend.SetPositions(segmentPoses);
    }
    void FixedUpdate()
    {
        if (doWiggle)
        {
            wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);
        }

        if (!isSegmented)
        {
            NormalSegment();
        }
        else
        {
            SegmentedBody();
        }
    }
    

}

