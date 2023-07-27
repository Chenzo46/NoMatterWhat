using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainGenerator : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rate;
    [SerializeField] private GameObject rainObject;
    [SerializeField] private float range;

    private List<RainObj> rainObjects;

    private float rateRef;

    private void Awake()
    {
        rateRef = rate;
    }

    private void Update()
    {
        if (rateRef > 0)
        {
            rateRef -= Time.deltaTime;
        }
        else
        {
            rateRef = rate;
            Vector2 spawnPos = new Vector2(Random.Range(transform.position.x - range, transform.position.x + range), transform.position.y);
            RainObj rj = Instantiate(rainObject, spawnPos, Quaternion.identity).GetComponent<RainObj>();
            rj.setSpeed(speed);
            //rainObjects.Add(rj);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position - new Vector3(range,0), transform.position + new Vector3(range,0));
    }
}
