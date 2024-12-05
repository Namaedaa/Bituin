using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Audio;

public static class SettingsLoader
{
    //Saving
    static string settings_name = "settings.sfg";
    static string file_path = $"{Application.persistentDataPath}\\{settings_name}";

    public static void saveSettings(float volume)
    {

        int resolution_width = Screen.currentResolution.width;
        int resolution_height = Screen.currentResolution.height;

        SettingsData to_save = new SettingsData
            (
            resolution_width: resolution_width,
            resolution_height: resolution_height,
            graphics: QualitySettings.GetQualityLevel(),
            fullscreen: Screen.fullScreen,
            volume: volume
            );

        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(file_path, FileMode.Create))
        {
            formatter.Serialize(stream, to_save);
            stream.Dispose();
        }
    }

    public static SettingsData loadSettings()
    {
        if (SettingsExists())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(file_path, FileMode.Open))
            {
                try
                {
                    SettingsData loaded_data = formatter.Deserialize(stream) as SettingsData;
                    return loaded_data;
                }
                catch (SerializationException)
                {
                    stream.Dispose();
                    return null;
                }
            }
        }
        else
        {
            Debug.Log("No Saved settings yet");
        }
        return null;
    }

    private static bool SettingsExists()
    {
        if (File.Exists(file_path))
            return true;
        return false;
    }
}

[System.Serializable]
public class SettingsData
{
    public int resolution_width;
    public int resolution_height;
    public int graphics;

    public bool fullscreen;
    public float volume;

    public SettingsData(int resolution_width, int resolution_height, int graphics, bool fullscreen, float volume)
    {
        this.resolution_width = resolution_width;
        this.resolution_height = resolution_height;
        this.graphics = graphics;
        this.fullscreen = fullscreen;
        this.volume = volume;
    }
}
