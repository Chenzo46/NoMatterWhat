using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamBackground : MonoBehaviour
{
    [SerializeField] private GameObject normal;
    [SerializeField] private GameObject disturbed;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private float distanceToStart = 8f;
    [SerializeField] private Animator anim;

    private Vector3 refSmooth;
    private bool fading = false;
    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, endPos.position - Vector3.forward*10, ref refSmooth, speed);

        if(!fading && Vector2.Distance(transform.position, endPos.position) <= distanceToStart)
        {
            StartCoroutine(swapScenes());
        }
    }

    private IEnumerator swapScenes()
    {
        fading = true;
        anim.SetTrigger("fadeOut");
        yield return new WaitForSeconds(2f);
        transform.position = startPos.position;
        disturbed.SetActive(!disturbed.activeSelf);
        normal.SetActive(!normal.activeSelf);
        anim.SetTrigger("fadeIn");
        yield return new WaitForSeconds(2f);
        fading = false;

    }


}
