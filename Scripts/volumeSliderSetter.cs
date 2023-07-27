using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class volumeSliderSetter : MonoBehaviour
{
    [SerializeField] private string saveKey;
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        volumeSlider.value = SaveDataManager.Singleton.gameStateVariables.getFloat(saveKey);
        volumeSlider.onValueChanged.AddListener((v) => GameStateManager.Singleton.updateVolumeSetting(v,saveKey));
    }
}
