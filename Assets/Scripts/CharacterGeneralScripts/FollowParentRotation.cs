using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParentRotation : MonoBehaviour
{

    Transform parentRotation;
    private Vector2 direction;
    [SerializeField] float speed;
    private void Start()
    {
        parentRotation = this.transform.parent;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        direction = parentRotation.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
    }
}
