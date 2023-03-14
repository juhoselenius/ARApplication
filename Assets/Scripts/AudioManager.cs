using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/*
 * @Author: Juho Selenius
 * With the help of "Introduction to AUDIO in Unity"
 * by Brackeys (https://www.youtube.com/watch?v=6OT43pvUyfY).
 */

public class AudioManager : MonoBehaviour
{
    public static AudioManager aManager;

    public Sound[] sounds;

    private void Awake()
    {
        if (aManager == null)
        {
            DontDestroyOnLoad(gameObject);
            aManager = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("BackgroundSound");
    }

    public void Play(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);
        if(s == null)
        {
            Debug.Log("Couldn't find the \"" + clipName + "\" sound from the sound list. Check spelling!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);
        if (s == null)
        {
            Debug.Log("Couldn't find the \"" + clipName + "\" sound from the sound list. Check spelling!");
            return;
        }
        s.source.Stop();
    }

    public void StopAll()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.source.isPlaying)
            {
                Debug.Log(SceneManager.GetActiveScene().name + " scene stops (StopPlayingAllSounds) sound: " + sound.source.clip.name);
                sound.source.Stop();
            }
        }
    }

    public Sound GetPlayingMusic()
    {
        foreach (Sound sound in sounds)
        {
            if (sound.source.isPlaying && sound.audioType == Sound.AudioTypes.music)
            {
                return sound;
            }
        }

        return null;
    }

    public bool IsClipPlaying(string clipName)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.source.isPlaying && sound.name == clipName)
            {
                return true;
            }
        }

        return false;
    }
}
