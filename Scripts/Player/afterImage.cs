using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class afterImage : MonoBehaviour
{
    [SerializeField] private GameObject image; 
    [SerializeField] private float frequency = 0.5f;

    private float freqRef;
    private SpriteRenderer spr;

    private void Awake()
    {
        freqRef = frequency;
        spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(freqRef <= 0)
        {
            Quaternion q = spr.flipX ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(0f, 0f, 0f);
            SpriteRenderer s = Instantiate(image, transform.position, q).GetComponent<SpriteRenderer>();
            s.sprite = s.sprite;
            Destroy(s.gameObject, 1f);
            freqRef = frequency;
        }
        else
        {
            freqRef -= Time.deltaTime;
        }
    }

    public void setAfterImage(GameObject img)
    {
        image = img;
    }

}
