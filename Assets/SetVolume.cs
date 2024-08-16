using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer master;

    public void Set(float volume)
    {
        master.SetFloat("MasterVolume", volume);
    }
}
