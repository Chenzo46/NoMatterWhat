using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private AudioClip click_sound;
    [SerializeField] private AudioClip letgo_sound;
    [SerializeField] private AudioClip hover_sound;

    private void Start() => musicTrackManager.Singleton.musicCollection.playSong("title");
    public void goToSettings()
    {
        anim.SetTrigger("toSettings");
    }

    public void goToMainFromSettings()
    {
        anim.SetTrigger("toMainFromSettings");
    }

    public void clickSound()
    {
        AudioManager.Singleton.playSound(click_sound, AudioManager.SoundType.SFX);
    }

    public void hoverSound()
    {
        AudioManager.Singleton.playSound(hover_sound, AudioManager.SoundType.SFX);
    }

    public void letgoSound() 
    {
        AudioManager.Singleton.playSound(letgo_sound, AudioManager.SoundType.SFX);
    }
}
