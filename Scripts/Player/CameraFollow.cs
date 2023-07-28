using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float damp = 0.2f;
    [SerializeField] private Vector2 offset = new Vector2();
    [SerializeField] private float offsetDamp = 0.1f;
    private Vector2 orgOffset = new Vector2();

    public static CameraFollow Singleton;

    private Vector3 smoothRef;
    private Vector2 offsetRef;

    private void Awake()
    {
        Singleton = this;
        orgOffset = offset;
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position,new Vector3(target.position.x + offset.x, target.position.y + offset.y, -10), ref smoothRef, damp);
        offset = Vector2.SmoothDamp(orgOffset, offset, ref offsetRef, offsetDamp);
    }

    public void setTarget(Transform tg)
    {
        target = tg;
        transform.position = tg.position;
    }

    public void editOffset(Vector2 translation)
    {
        offset += translation;
    }
}
