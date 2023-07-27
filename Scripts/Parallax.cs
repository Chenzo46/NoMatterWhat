using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private GameObject cam;
    private float startPos;
    private float length;

    [SerializeField] private float parallaxEffect;

   
    private void Start()
    {
        cam = Camera.main.gameObject;
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distance = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + distance, cam.transform.position.y, transform.position.z);

        if (temp > startPos + length)
        {
            startPos += length;
        }
        else if (temp < startPos - length)
        {
            startPos -= length;
        }
    }



}
