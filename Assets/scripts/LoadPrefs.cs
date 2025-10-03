using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;    

public class LoadPrefs : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private GameMenu menuController;

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;


    [Header("Brightness Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;


    [Header("Quality Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown = null;

    [Header("Fullscreen Settings")]
    [SerializeField] private Toggle fullScreenToggle;


    [Header("Sensitivity Settings")]
    [SerializeField] private TMP_Text ControllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;


    [Header("Invert Y Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    private void Aake()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");

                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }
            else
            {
                menuController.ResetButton("Audio");    
            }
            if(PlayerPrefs.HasKey("qualityLevel"))
            {
                int localQuality = PlayerPrefs.GetInt("qualityLevel");
                qualityDropdown.value = localQuality;
                QualitySettings.SetQualityLevel(localQuality);
            }
            else
            {
                menuController.ResetButton("Graphics");
            }
            if (PlayerPrefs.HasKey("isFullScreen")){
                int localFullscreen = PlayerPrefs.GetInt("isFullScreen");
                if (localFullscreen == 1)
                {
                    fullScreenToggle.isOn = true;
                    Screen.fullScreen = true;
                }
                else
                {
                    fullScreenToggle.isOn = false;
                    Screen.fullScreen = false;
                }
                if(PlayerPrefs.HasKey("brightness"))
                {
                    float localBrightness = PlayerPrefs.GetFloat("brightness");
                    brightnessTextValue.text = localBrightness.ToString("0.0");
                    brightnessSlider.value = localBrightness;
                }
                else
                {
                    menuController.ResetButton("Graphics");
                }
                if (PlayerPrefs.HasKey("controllerSen"))
                {
                    float localControllerSen = PlayerPrefs.GetFloat("controllerSen");
                    ControllerSenTextValue.text = localControllerSen.ToString("0");
                    controllerSenSlider.value = localControllerSen;
                    menuController.mainControllerSen = Mathf.RoundToInt(localControllerSen);    
                }
                else
                {
                    menuController.ResetButton("Controls");
                }
                if (PlayerPrefs.HasKey("invertY"))
                {
                  
                    if (PlayerPrefs.GetInt("invertY")== 1)
                    {
                        invertYToggle.isOn = true;
                        
                    }
                    else
                    {
                        invertYToggle.isOn = false;
                        
                    }
                }
                else
                {
                    menuController.ResetButton("Controls");
                }
            }
        }
    }

}
