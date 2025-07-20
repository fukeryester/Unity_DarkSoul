using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider Player_Capsule;
    public float offset = 0.5f;
    private Vector3 point1;
    private Vector3 point2;
    private float radius;

    // Start is called before the first frame update
    void Awake()
    {
        radius = Player_Capsule.radius - 0.1f;
    }
    private void FixedUpdate()
    {
        // get two points to define the capsule surrounding player
        point1 = transform.position + transform.up * (Player_Capsule.radius - offset);
        point2 = transform.position + transform.up * (Player_Capsule.height - offset) - transform.up * Player_Capsule.radius;  
        Collider[] colliders = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));
        if (colliders.Length > 0)
        {
            SendMessageUpwards("isOnGround");
        }
        else SendMessageUpwards("isNotOnGround");
    }

}
