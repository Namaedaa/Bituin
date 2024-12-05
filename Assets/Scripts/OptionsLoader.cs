using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsLoader : MonoBehaviour
{
    public SettingsScript settingsObject;
    public GameObject MainMenuBack;
    public GameObject IngameBack;

    private void OnEnable()
    {
        if(InGameMenu.inGame)
        {
            IngameBack.SetActive(true);
            MainMenuBack.SetActive(false);
        }
        else
        {
            IngameBack.SetActive(false);
            MainMenuBack.SetActive(true);
        }
         loadSettings();
      
    }

    public void loadSettings()
    {
        SettingsData loaded_settings = SettingsLoader.loadSettings();
        if( loaded_settings == null ) 
            return;
    

        Screen.SetResolution(loaded_settings.resolution_width, loaded_settings.resolution_height, loaded_settings.fullscreen,60);
        Resolution[] resolutions = settingsObject.resolutions;

        int resolution_index = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == loaded_settings.resolution_width &&
                resolutions[i].height == loaded_settings.resolution_height)
            {
                resolution_index = i;
                break;
            }
        }

        settingsObject.resolutionDropdown.value = resolution_index;

        settingsObject.graphicsDropdown.value = loaded_settings.graphics;
        QualitySettings.SetQualityLevel(loaded_settings.graphics);

        settingsObject.fullscreenToggle.isOn = loaded_settings.fullscreen;

        settingsObject.volumeSlider.value = loaded_settings.volume;
        settingsObject.audioMixer.SetFloat("MasterVolume", loaded_settings.volume);
    }
}
