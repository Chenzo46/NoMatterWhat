using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource ad_sfx;
    
    public float master_volume { get; private set; } = 0.5f;
    public float music_volume { get; private set; } = 0.5f;
    public float sfx_volume { get; private set; } = 0.5f;

    public static AudioManager Singleton;

    public enum SoundType
    {
        Music,
        SFX,
    }

    private void Awake()
    {
        if(Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        master_volume = SaveDataManager.Singleton.gameStateVariables.getFloat("master_volume");
        sfx_volume = SaveDataManager.Singleton.gameStateVariables.getFloat("sound_effects");
        music_volume = SaveDataManager.Singleton.gameStateVariables.getFloat("music_volume");
        AudioListener.volume = master_volume;
    }

    public void playSound(AudioClip ac, SoundType type)
    {
        if(type == SoundType.SFX)
        {
            ad_sfx.volume = sfx_volume;
            ad_sfx.PlayOneShot(ac);
        }
        
    }

    public void setMasterVolume(float volume)
    {
        master_volume = volume;
        AudioListener.volume = volume;
    }

    public void setSFXVolume(float volume)
    {
        sfx_volume = volume;
    }
}
