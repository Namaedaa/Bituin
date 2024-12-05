using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class SettingsScript : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;

    public TMP_Dropdown graphicsDropdown;
    
    public Toggle fullscreenToggle;

    public Slider volumeSlider;

    internal Resolution[] resolutions;

    public OptionsLoader optionsLoader;


    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        HashSet<string> options = new HashSet<string>();
        List<Resolution> temp_res = new List<Resolution> ();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if(!options.Contains(option))
            {
                options.Add(option);
                Resolution temporary = new Resolution();
                temporary.width = resolutions[i].width;
                temporary.height = resolutions[i].height;
                temporary.refreshRate = 60;
                temp_res.Add(temporary);
            }
        }

        resolutions = temp_res.ToArray();

        resolutionDropdown.AddOptions(options.ToList());
        resolutionDropdown.RefreshShownValue();

        

        optionsLoader.loadSettings();
        DontDestroyOnLoad(this.gameObject);
    }


    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
        Debug.Log("Volume: " + volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution()
    {
        int resolutionIndex = resolutionDropdown.value;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen,60);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void save_settings()
    {
        float value;
        audioMixer.GetFloat("MasterVolume", out value);
        SettingsLoader.saveSettings(value);
    }
}


