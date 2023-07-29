using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CurrentDeviceVisual : MonoBehaviour
{
    [SerializeField] private Sprite Gamepad;
    [SerializeField] private Sprite Keyboard;
    [SerializeField] private Image img;
    private void OnEnable()
    {
        controllerManager.Singleton.onDeviceChanged += changeDeviceIcon;
    }
    private void OnDisable()
    {
        controllerManager.Singleton.onDeviceChanged -= changeDeviceIcon;
    }

    private void changeDeviceIcon(controllerManager.ControllerType type)
    {
        if (type == controllerManager.ControllerType.Gamepad)
        {
            img.sprite = Gamepad;
        }
        else if (type == controllerManager.ControllerType.Keyboard)
        {
            img.sprite = Keyboard;
        }
    }
}
