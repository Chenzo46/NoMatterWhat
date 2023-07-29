using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiSoundInputs : MonoBehaviour
{
    private DefaultInputActions inputActions = null;

    [SerializeField] private AudioClip hover;
    [SerializeField] private AudioClip submit;

    private void Awake()
    {
        inputActions = new DefaultInputActions();
    }

    private void OnEnable()
    {
        subscribeEvents();
    }

    private void OnDisable()
    {
        unsubscribeEvents();
    }

    private void subscribeEvents()
    {
        inputActions.UI.Navigate.performed += playHoverSound;
        inputActions.UI.Submit.performed += playSubmitSound;
    }

    private void unsubscribeEvents()
    {
        inputActions.UI.Navigate.performed -= playHoverSound;
        inputActions.UI.Submit.performed -= playSubmitSound;
    }

    private void playHoverSound(InputAction.CallbackContext value)
    {
        AudioManager.Singleton.playSound(hover, AudioManager.SoundType.SFX);
    }
    private void playSubmitSound(InputAction.CallbackContext value)
    {
        AudioManager.Singleton.playSound(submit, AudioManager.SoundType.SFX);
    }


}
