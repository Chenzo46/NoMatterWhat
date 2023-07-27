using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class boxBehavior : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask liquid;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float maxVelocityInLiquid;
    [SerializeField] private BoxCollider2D bx;
    private Animator anim;

    private bool pickedUp = false;
    private Transform pickupLocation;

    private Vector2 smoothRef;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    

    private bool isInLiquid()
    {
        return Physics2D.OverlapBox(transform.position, new Vector2(0.9f,0.9f), 0f, liquid);
    }

    private void FixedUpdate()
    {
        if (isInLiquid())
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocityInLiquid);
        }
        else
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
        }



    }

    private void Update()
    {
        if (pickedUp)
        {
            //rb.MovePosition(pickupLocation.position);
            transform.position = pickupLocation.position;
            rb.angularVelocity = 0f;
            rb.MoveRotation(Quaternion.Euler(0f,0f,0f));
        }
    }

    public void applyForce(Vector2 dir)
    {
        pickedUp = false;
        rb.gravityScale = 3f;
        rb.AddForce(dir, ForceMode2D.Impulse);
    }

    public void setPickupLocation(Transform loc)
    {
        pickupLocation = loc;
        pickedUp = true;
        rb.gravityScale = 0f;
    }

    public void dropBox()
    {
        pickedUp = false;
        rb.gravityScale = 3f;
    }

    public void consumeBox()
    {
        anim.SetTrigger("consume");
    }

}
