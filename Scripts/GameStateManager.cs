using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SaveDataManager saveDataManager;
    [SerializeField] private musicTrackManager trackManager;

    public static GameStateManager Singleton;
    private void Awake()
    {
        if (Singleton != null) 
        { 
            Destroy(gameObject); 
        }
        else
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }        
    }
    private void Start()
    {
        setSoundVolumeFromSave();
    }

    #region Volume Setting Manager
    public void updateVolumeSetting(float sl_value, string save_key)
    {
        if (save_key.Equals("master_volume"))
        {
            audioManager.setMasterVolume(sl_value);
        }
        else if (save_key.Equals("sound_effects"))
        {
            audioManager.setSFXVolume(sl_value);
        }
        else if (save_key.Equals("music_volume"))
        {
            trackManager.musicCollection.changeCurrentSongVolume(sl_value);
        }

        saveDataManager.gameStateVariables.addKey(save_key, sl_value);
        saveDataManager.saveData();
    }
    private void setSoundVolumeFromSave()
    {
        audioManager.setMasterVolume(saveDataManager.gameStateVariables.getFloat("master_volume"));
        trackManager.musicCollection.changeCurrentSongVolume(saveDataManager.gameStateVariables.getFloat("music_volume"));
        audioManager.setSFXVolume(saveDataManager.gameStateVariables.getFloat("sound_effects"));
    }

    #endregion


}
