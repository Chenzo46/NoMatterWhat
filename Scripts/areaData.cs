using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class areaData : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject alternate;

    [SerializeField] private GameObject[] extraToggles;

    public void toggleMatter()
    {
        main.SetActive(!main.activeSelf);
        alternate.SetActive(!alternate.activeSelf);
        toggleExtras();
    } 

    private void toggleExtras()
    {
        if (extraToggles.Length < 1) return;
        foreach (GameObject g in extraToggles)
        {
            g.SetActive(!g.activeSelf);
        }
    }
}
