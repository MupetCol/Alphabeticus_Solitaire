using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongStateHandler : MonoBehaviour
{
    private AudioSource source;
	private float maxVolume;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		maxVolume = source.volume;
	}

	public void SwitchState()
	{
		//Mute/Unmute according to volume
		if(source.volume == maxVolume)
		{
			source.volume = 0f;
		}
		else
		{
			source.volume = maxVolume;
		}
	}
}
