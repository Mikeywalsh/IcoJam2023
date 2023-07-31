using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [SerializeField] private Sound[] sounds;
    private readonly IDictionary<string, Sound> _soundsDictionary = new Dictionary<string, Sound>();

    public static bool VolumeMuted;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var sound in sounds)
        {
            _soundsDictionary.Add(sound.name, sound);
        }

        Play("main-theme");
    }

    private void FixedUpdate()
    {
        TryPlay("main-theme");
    }

    public static void ToggleMute()
    {
        VolumeMuted = !VolumeMuted;
        AudioListener.volume = VolumeMuted ? 0 : 1f;
    }

    public static void Play(string name)
    {
        if (_instance._soundsDictionary.TryGetValue(name, out var sound))
        {
            sound.Play();
        } 
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    public static void TryPlay(string name)
    {
        if (_instance._soundsDictionary.TryGetValue(name, out var sound))
        {
            if (!sound.source.isPlaying)
            {
                sound.Play();
            }
        } 
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    public static void Stop(string name)
    {
        if (_instance._soundsDictionary.TryGetValue(name, out var sound))
        {
            sound.source.Stop();
        } 
        else
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
    }

    public static bool IsPlaying(GameObject gameObject)
    {
        return gameObject.GetComponent<AudioSource>() != null && gameObject.GetComponent<AudioSource>().isPlaying;
    }

    public static void SetPitch(float pitch)
    {
        foreach (var sound in _instance._soundsDictionary.Values)
        {
            sound.pitch = pitch;
        }
    }
}