using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private UnityEvent OnInteracted;
    [SerializeField] private int priority = 0;
    [SerializeField] private List<MatterSwitcher.PlayerState> allowedStates;
    public void invokeInteracted(MatterSwitcher.PlayerState current)
    {

        if (allowedStates.Contains(current))
        {
            OnInteracted.Invoke();
        }
    }
    public int getPriority()
    {
        return priority;
    }
    public List<MatterSwitcher.PlayerState> getAllowedSates()
    {
        return allowedStates;
    }
}
