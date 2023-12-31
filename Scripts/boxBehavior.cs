using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class boxBehavior : MonoBehaviour
{
    //Start of box branch
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask liquid;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float maxVelocityInLiquid;
    [SerializeField] private BoxCollider2D bx;
    private Animator anim;

    public delegate void BoxActions();
    public event BoxActions OnBoxAbsorbed;

    private bool pickedUp = false;
    private Transform pickupLocation;

    private Vector2 bxSize;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        bxSize = bx.size;
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
            rb.velocity = Vector2.zero;
            rb.MoveRotation(Quaternion.Euler(0f,0f,0f));
        }
    }

    public void applyForce(Vector2 dir)
    {
        pickedUp = false;
        rb.gravityScale = 3f;
        rb.AddForce(dir, ForceMode2D.Impulse);
        bx.enabled = true;
        bx.size = bxSize;
    }

    public void setPickupLocation(Transform loc)
    {
        pickupLocation = loc;
        pickedUp = true;
        rb.gravityScale = 0f;
        bx.size = bxSize/2;
        bx.enabled = false;
    }

    public void dropBox()
    {
        pickedUp = false;
        bx.enabled = true;
        rb.gravityScale = 3f;
        bx.size = bxSize;
    }

    public void consumeBox()
    {
        anim.SetTrigger("consume");
        OnBoxAbsorbed();
    }

}
