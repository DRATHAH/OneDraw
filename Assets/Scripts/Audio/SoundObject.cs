using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    [Tooltip("Variance in how high/low the pitch of the sound played is.")]
    public float pitchVariance = 0.1f;
    public AudioSource sound;

    public void Initialize(AudioSource soundToPlay)
    {
        sound.clip = soundToPlay.clip;
        sound.rolloffMode = soundToPlay.rolloffMode;
        sound.volume = soundToPlay.volume;
        float pitch = Random.Range(pitchVariance-pitchVariance, pitchVariance);
        sound.pitch = soundToPlay.pitch + pitch;

        sound.Play();
        StartCoroutine(DeleteAfterTime());
    }

    IEnumerator DeleteAfterTime()
    {
        float waitTime = 1;
        if (sound.clip)
        {
            waitTime += sound.clip.length;
        }
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
