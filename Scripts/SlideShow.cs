using UnityEngine;
using System.Collections;

public class SlideShow : MonoBehaviour {
	public Texture2D[] slides;
	private int page;
	private AudioSource audio;
	void Start () {
		page = 0;
		audio = this.gameObject.GetComponent<AudioSource>();
	}

	void Update () {
		if(Input.GetMouseButtonUp(0)){
			page++;
		}
		if(page < slides.Length){
			audio.Pause();
			audio.clip = SoundManager.music[page];
			audio.Play();
		}

		if(page >= slides.Length){
			Application.LoadLevel(1);
		}
	}

	void OnGUI(){

		if(page < slides.Length){
			GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), slides[page]);
		}
	}
}
