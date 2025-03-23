using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        SetPitch(1f);
    }
    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("master Volume", Mathf.Log10(level)*20f);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("music Volume", Mathf.Log10(level) * 20f);
    }
    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("sfx Volume", Mathf.Log10(level) * 20f);
    }

    public void SetPitch(float level)
    {
        audioMixer.SetFloat("music Pitch", level);
    }
}
