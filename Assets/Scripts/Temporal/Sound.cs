using System;
using UnityEngine;


[Serializable]
public class Sound
{
    public AudioClip clip;
    public AudioSource source;

    public bool loop;
    public string name;

    [Range(.1f, 3f)] public float pitch = 1f;

    [Range(0f, 1f)] public float volume = 1f;

    public void Play()
    {
        source.Stop();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.Play();
    }
}