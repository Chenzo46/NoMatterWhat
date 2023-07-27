using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float speed = 5f;

    private Vector2 direction;

    private void Awake()
    {
    }

    private void Update()
    {
        direction = transform.right;

        transform.localPosition = direction * moveDistance * Mathf.Sin(Time.time * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            collision.GetComponent<MatterSwitcher>().killPlayer();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position + transform.right * moveDistance, transform.position - transform.right * moveDistance);
    }
}
