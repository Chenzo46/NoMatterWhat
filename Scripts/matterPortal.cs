using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class matterPortal : MonoBehaviour
{
    [SerializeField] private bool isActivated = true;
    [SerializeField] private LayerMask box;
    [SerializeField] private Transform fishPos;
    private BoxCollider2D bx;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        bx = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        anim.SetBool("active", isActivated);

        checkForBox();
    }

    private void checkForBox()
    {
        if (isActivated) return;

        Collider2D boxRef = Physics2D.OverlapBox(transform.position, bx.bounds.size, 0f, box);

        if (boxRef != null) 
        {
            boxBehavior temp = boxRef.GetComponent<boxBehavior>();
            temp.consumeBox();
            temp.setPickupLocation(transform);

            isActivated = true;
        }
    }

    public bool getActiveState()
    {
        return isActivated;
    }

    public Vector3 getFishPos()
    {
        return fishPos.position;
    }
}
