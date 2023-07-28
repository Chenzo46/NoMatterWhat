using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainGUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] worldLayers;
    [SerializeField] private TMP_Text[] levelLayers;
    private void Awake()
    {
        setText();
    }
    private void setText()
    {
        foreach(TMP_Text tt in levelLayers)
        {
            tt.text = $"Level {SceneManager.GetActiveScene().buildIndex}";
        }
        foreach(TMP_Text tt in worldLayers)
        {
            tt.text = $"World {SceneManager.GetActiveScene().name.Substring(0, 2)[1]}";
        }
    }

    public void playButtonSound(AudioClip cl)
    {
        try
        {
            AudioManager.Singleton.playSound(cl, AudioManager.SoundType.SFX);
        }
        catch
        {

        }
    }

    public void goToMainMenu()
    {
        try
        {
            musicTrackManager.Singleton.musicCollection.fadeOutSong(0.35f);
        }
        catch 
        {
        }
        
        SceneTransitioner.Singleton.toMainMenu();
    }
}
