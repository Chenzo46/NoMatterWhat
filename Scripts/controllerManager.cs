using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class controllerManager : MonoBehaviour
{
    public ControllerType currentController { get; private set; } = ControllerType.Keyboard;
    public static controllerManager Singleton;

    public delegate void inputDelegate(ControllerType controllerType);
    public event inputDelegate onDeviceChanged;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    private void Update()
    {
        /*if (Gamepad.current.wasUpdatedThisFrame && currentController != ControllerType.Gamepad)
        {
            currentController = ControllerType.Gamepad;
            onDeviceChanged(currentController);
        }
        else if ((Keyboard.current.anyKey.wasPressedThisFrame) && currentController != ControllerType.Keyboard)
        {
            currentController = ControllerType.Keyboard;
            onDeviceChanged(currentController);
        }*/
    }

    public enum ControllerType
    {
        Keyboard,
        Gamepad
    }


}
