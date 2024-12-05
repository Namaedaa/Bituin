using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicChangerManager : MonoBehaviour
{
    [SerializeField] Music[] musicSources;
    private string currentMusicID;
    [SerializeField] AudioMixerGroup audioMixerGroup;
    
    
    // Start is called before the first frame update
    void Awake()
    {
        
        foreach (Music source in musicSources) 
        { 
            source.source = gameObject.AddComponent<AudioSource>();
            source.source.clip = source.clip;
            source.source.loop = true;
            source.source.outputAudioMixerGroup = audioMixerGroup;
        
        }
        MusicPlay("Normal");
    }

    public void MusicPlay(string musicId)
    {
        MusicStop();
        currentMusicID = musicId;

        foreach (Music source in musicSources)
        {
            if (source.musicID == currentMusicID)
            {
                source.source.Play();
                break;
            }
        }
    }
    public void MusicStop()
    {
        foreach (Music source in musicSources)
        {
            if (source.musicID == currentMusicID)
            {
                source.source.Stop();
                break;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
