
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(AudioSource))]
public class SoundManager {
	public static List<AudioClip> music;
	public static List<AudioClip> attackSounds;
	public static List<AudioClip> soundEffects;
	
	static SoundManager(){
		music = new List<AudioClip>();
		attackSounds = new List<AudioClip>();
		soundEffects = new List<AudioClip>();

		AudioClip theme = Resources.Load("Sounds/level_theme") as AudioClip;

		attackSounds.Add(Resources.Load("Sounds/back_slash") as AudioClip);
		attackSounds.Add(Resources.Load("Sounds/typhoon_swing") as AudioClip); 
		attackSounds.Add(Resources.Load("Sounds/lion_charge") as AudioClip); 

		soundEffects.Add(Resources.Load("Sounds/health_pickup") as AudioClip); 
		soundEffects.Add(Resources.Load("Sounds/starting_call") as AudioClip);
		soundEffects.Add(Resources.Load("Sounds/death_scream") as AudioClip);
		soundEffects.Add(Resources.Load("Sounds/wave_clear") as AudioClip);
		soundEffects.Add(Resources.Load("Sounds/wave_start") as AudioClip);

		music.Add(Resources.Load("Sounds/titlescreen") as AudioClip);
		music.Add(theme);
		music.Add(Resources.Load("Sounds/titlescreen") as AudioClip);
	}
}

