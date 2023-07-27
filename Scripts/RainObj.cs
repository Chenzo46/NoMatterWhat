using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainObj : MonoBehaviour
{
    [SerializeField] private LayerMask stopper;
    [SerializeField] private LayerMask slowdown;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private Animator anim;
    private float speed = 1f;
    private bool hasHit = false;
    public enum RainType
    {
        Water, Ice
    }

    public static RainType currentRainType = RainType.Water;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        anim.SetBool("isWater", currentRainType == RainType.Water);
        if (hasHit) { return; }

        if (isCollidingWithStopper())
        {
            anim.SetTrigger("hit");
            Destroy(gameObject,2f);
        }
        else if(!isCollidingWithStopper())
        {
            transform.position -= Vector3.up * speed * Time.deltaTime;
        }

        
    }

    public static void toggleType()
    {
        if(currentRainType == RainType.Water)
        {
            currentRainType = RainType.Ice;
        }
        else
        {
            currentRainType = RainType.Water;
        }
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    private bool isCollidingWithStopper()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.23f, Vector2.up, 0.23f, stopper);
        if(hit.collider != null)
        {
            transform.position = hit.point;
            hasHit = true;
        }

        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") && currentRainType == RainType.Ice)
        {
            collision.GetComponent<MatterSwitcher>().killPlayer();
            anim.SetTrigger("hit");
            Destroy(gameObject, 2f);
        }
    }
}
