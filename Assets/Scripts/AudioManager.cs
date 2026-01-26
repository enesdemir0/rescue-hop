using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        // SINGLETON PATTERN:
        // This ensures there is only ONE AudioManager, and it never dies.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // <--- THIS IS THE FIX
        }
        else
        {
            // If we load a menu and there is already an AudioManager from before,
            // destroy the new one so we don't have duplicates.
            Destroy(gameObject);
        }
    }


    public AudioSource[] music;
    public AudioSource[] sfx;

    public void PlayMusic(int TrackNumber)
    {
        StopMusic();
        if(TrackNumber < music.Length)
        {
            music[TrackNumber].Play();
        }
    }

    public void StopMusic()
    {
        foreach(AudioSource track in music)
        {
            track.Stop();
        }
    }
    public void PlaySFX(int soundToPlay)
    {
        if (soundToPlay < sfx.Length)
        {
            sfx[soundToPlay].Stop();
            sfx[soundToPlay].Play();
        }
    }

    public void PlaySFXPitched(int soundToPlay)
    {
        sfx[soundToPlay].pitch = Random.Range(.8f, 1.2f);
        PlaySFX(soundToPlay);

    }
    
}
