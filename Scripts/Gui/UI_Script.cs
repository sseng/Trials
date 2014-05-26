using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IconTextures{
	public Texture2D healthy;
	public Texture2D lowHealth;
	public Texture2D slashOn;
	public Texture2D slashOff;
	public Texture2D spinOn;
	public Texture2D spinOff;
	public Texture2D chargeOn;
	public Texture2D chargeOff;
	public Texture2D primeOn;
	public Texture2D primeOff;
}
[System.Serializable]
public class PauseMenu{
	public Rect frame;
	public string[] text;
	public GUIStyle menuStyle;
}

[System.Serializable]
public class Combo{
	public Rect frame;
	public GUIStyle style;
}
[System.Serializable]
public class OrthoCam{
	public int size = 40;
	public int minSize = 20;
	public int maxSize = 70;
}

public class UI_Script : MonoBehaviour {
	static public int score = 0;
	static public float multiplier = 1;
	static public int killCombo = 0;
	static public float killComboTimer = 0;
	public int lowThreshold;
	public List<Texture2D> Numbers;
	public IconTextures icons;
	public Texture2D title;
	public bool titleScreenOn;
	public GUIStyle scoreStyle;
	public Rect ScoreFrame;
	public Rect playerFrame;
	public Rect healthbarFrame;
	public Rect	layoutFrame;
	public Rect abilitiesFrame;
	public Vector2 abilitiesOffset;
	public PauseMenu pauseMenu;
	public Combo comboAlert;
	public OrthoCam orthoCam;

	private float spinTime;
	private float slashTime;
	private float chargeTime;
	private float currentHp;
	private float maxHp;

	private float redVar = 0;
	private float greenVar = 1;
	private AudioSource soundSource;
	private WaveGenerator waveGen;
	private Color hpColor;
	private Vector2 defaultScreenRes;
	private bool displayLabel;
	private UnitManager UM; //to get player hp values.
	private bool paused;
	private bool prime;

	void Start () {
		soundSource = this.gameObject.GetComponent<AudioSource>();
		//waveGen = GameObject.FindWithTag("Wave").GetComponent<WaveGenerator>();
		UM = GameObject.FindWithTag("Player").GetComponent<UnitManager>();
		titleScreenOn = true;
		defaultScreenRes.x = 1024f;
		defaultScreenRes.y = 768f;

	}

	void Update (){
		currentHp = UM.getUnit().curHealth;
		maxHp = UM.getUnit().maxHealth;
		lowThreshold = (int)(maxHp/3f);

		CameraZoom(); //mouse scroll zooming

		//speed up theme music when hp at lowthreshold
		if(soundSource.clip == SoundManager.music[1]){
			themePitch();
		}

		//Pause mechanics
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if(Time.timeScale == 1.0F){
				pauseGame ();
			}else{
				unPauseGame();
			}
			Time.fixedDeltaTime = 0.02F * Time.timeScale;
		}



		if (killComboTimer > 0) killComboTimer -= Time.deltaTime;
		if (killComboTimer <= 0) killCombo = 0;
	}

	void OnGUI() {
		TitleScreenEnable();
		ResizeGUIMatrix ();
		if(!titleScreenOn){
			//WaveTimerUI(waveGen.Wavetimer);
			DrawPlayerFrame(playerFrame, currentHp, lowThreshold);
			DrawHealthMeter(healthbarFrame, currentHp, maxHp);
			DrawAbilities(abilitiesFrame);
			DisplayScore(score, ScoreFrame, scoreStyle);
			DisplayCombos(killCombo, comboAlert.frame, comboAlert.style);

			if(paused == true){
				DrawPauseMenu();
			}

		}
		GUI.matrix = Matrix4x4.identity;
	}
	//function to render frame texture based on player's health.
	void DrawPlayerFrame(Rect frame, float currentHealth, float hpThreshold){

		if (currentHealth > hpThreshold){
			GUI.DrawTexture(frame, icons.healthy, ScaleMode.StretchToFill, true, 10.0f);
		}else{
			GUI.DrawTexture(frame, icons.lowHealth, ScaleMode.StretchToFill, true, 10.0f);
		}
	}

	void DrawHealthMeter(Rect frame, float curHP, float maxHP){
		float hpPercent = curHP/maxHP;
		frame.width = frame.width * hpPercent;
		greenVar = 1.0f * hpPercent;
		redVar = 1.0f - greenVar;
		hpColor = new Color(redVar, greenVar, 0);
		if (curHP > 0) {
			DrawQuad (frame, hpColor);
		}
	}
	
	void DrawQuad(Rect position, Color color) {
		Texture2D texture = new Texture2D(1, 1);
		texture.SetPixel(0,0,color);
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		GUI.Box(position, GUIContent.none);
	}

	void DrawAbilities(Rect frame){
		Texture2D slashTexture = icons.slashOff;
		Texture2D spinTexture = icons.spinOff;
		Texture2D chargeTexture = icons.chargeOff;
		int spinCD=0;
		int chargeCD=0;

		foreach (Ability ab in UM.getAbilities()) {
			switch(ab.abilityName){
			case "Slash":
				if(ab.CoolDown > 0)	slashTexture = icons.slashOff;
				else slashTexture = icons.slashOn;
				break;
			case "Spin":
				spinCD = Mathf.RoundToInt(ab.CoolDown);
				if (ab.CoolDown > 0) spinTexture = icons.spinOff;
				else spinTexture = icons.spinOn; 
				break;
			case "Charge":
				chargeCD = Mathf.RoundToInt(ab.CoolDown);
				if (ab.CoolDown > 0) chargeTexture = icons.chargeOff; 
				else chargeTexture = icons.chargeOn;
				break;
			}
		}

		GUI.BeginGroup(layoutFrame,"");

		GUI.DrawTexture(new Rect(frame.x, frame.y, frame.width, frame.height), slashTexture); //slash icon
		GUI.DrawTexture(new Rect(frame.x, (frame.height +abilitiesOffset.y)*1 +frame.y , frame.width, frame.height), spinTexture); //spin icon
		if(spinCD > 0)GUI.DrawTexture(new Rect(frame.x, frame.height +frame.y +abilitiesOffset.y, frame.width, frame.height), Numbers[spinCD]); //spin cd icon
		GUI.DrawTexture(new Rect(frame.x, (frame.height +abilitiesOffset.y)*2 +frame.y, frame.width, frame.height), chargeTexture); //charge icon
		if(chargeCD > 0)GUI.DrawTexture(new Rect(frame.x, (frame.height +abilitiesOffset.y)*2 +frame.y , frame.width, frame.height), Numbers[chargeCD]); //charge cd icon
		GUI.DrawTexture(new Rect(frame.x, (frame.height +abilitiesOffset.y)*3 +frame.y, frame.width, frame.height), icons.primeOff); //omega icon
		if(prime)GUI.DrawTexture(new Rect(frame.x, (frame.height +abilitiesOffset.y)*3 +frame.y, frame.width, frame.height), icons.primeOn);
		GUI.EndGroup();
	}

	void TitleScreenEnable(){
		if (titleScreenOn) {			
			if (Input.GetMouseButton (0)) {
				ToggleTitle ();
				Time.timeScale = 1.0f;
				soundSource.Pause ();
				soundSource.volume = 0.2f;
				soundSource.clip = SoundManager.music[1];
				soundSource.Play ();
			}
			else {
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), title );
				Time.timeScale = 0.0f;

				if (!soundSource.isPlaying){
					soundSource.volume = 1.0f;
					soundSource.clip = SoundManager.music[0];
					soundSource.Play();
				}
			}
		}
	}

	void ToggleTitle(){
		if (titleScreenOn) {
			titleScreenOn = false;
		}
		else{
			titleScreenOn = true;
		}
	}

	void DisplayScore(int score, Rect frame, GUIStyle style){
		GUI.Label(frame, "Score: " +score, style);

	}

	void DisplayCombos(int kills, Rect frame, GUIStyle style){
		if (killCombo >= 2) {
			StartCoroutine("flashingText");
			if(displayLabel == true){
				GUI.Label(frame, "" +killCombo +"  KILL COMBO! ", style);
			}
		}
	}

	IEnumerator flashingText(){
		displayLabel = true;
		yield return new WaitForSeconds(.5f);
		displayLabel = false;
		yield return new WaitForSeconds (.5f);
	}

	void ResizeGUIMatrix()
	{
		float scale;
		// Set matrix
		Vector2 ratio = new Vector2(Screen.width/defaultScreenRes.x , Screen.height/defaultScreenRes.y );
		Matrix4x4 guiMatrix = Matrix4x4.identity;
		if (ratio.x > ratio.y)
			scale = ratio.y;
		else 
			scale = ratio.x;

		guiMatrix.SetTRS(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(scale, scale, 1));
		GUI.matrix = guiMatrix;
	}

	static public void addScore(float kills){
		killCombo += 1;
		multiplier = Mathf.Floor(0.125f*killCombo*killCombo)/2+1;
		score += (int)(kills*multiplier);
		killComboTimer = 5.0f;
	}

	void pauseGame(){
		Time.timeScale = 0.0F;
		soundSource.Pause();
		paused = true;
	}

	void unPauseGame(){
		Time.timeScale = 1.0F;
		soundSource.Play();
		paused = false;
	}

	void DrawPauseMenu(){
		//GUI.color = Color.Lerp(Color.white, Color.black, Time.time);
		Color color = new Color(0,0,0,0.5f);
		DrawQuad(new Rect(0,0, Screen.width*2, Screen.height*2), color);
		//GUI.DrawTexture (new Rect (0, 0, Screen.width*2, Screen.height*2), screen);
		if (GUI.Button (pauseMenu.frame, pauseMenu.text[0], pauseMenu.menuStyle)) {
			//Application.LoadLevel(0);
		}
	}

	void CameraZoom(){
		if(Camera.main.orthographic == true){
			if(Input.GetAxis("Mouse ScrollWheel") < 0){
				orthoCam.size++;
			}
			if(Input.GetAxis("Mouse ScrollWheel") > 0){
				orthoCam.size--;
			}
			orthoCam.size = Mathf.Clamp(orthoCam.size, orthoCam.minSize, orthoCam.maxSize);
			Camera.main.orthographicSize = orthoCam.size;
		}
	}

	void themePitch(){
		float pitch = soundSource.pitch;

		if(currentHp <= lowThreshold){
			pitch += 0.025f * Time.deltaTime;
		}else{
			pitch -= 0.025f * Time.deltaTime;
		}
		pitch = Mathf.Clamp(pitch, 1f, 1.1f);
		soundSource.pitch = pitch;
	}
	/*
	void WaveTimerUI(float waveTimer){
		float minutes = 0;
		if(minutes>0){
			//GUI.DrawTexture(new Rect(0,10,60,60), Numbers[(waveTimer/60)], ScaleMode.StretchToFill, true, 10.0f);
			GUI.DrawTexture(waveTimerLocation, Numbers[(waveTimer/60)], ScaleMode.StretchToFill, true, 10.0f);
		}
		GUI.DrawTexture(new Rect(waveTimerLocation.x + 50,waveTimerLocation.y,waveTimerLocation.width, waveTimerLocation.height), 
		                Numbers[(waveTimer/10)], ScaleMode.StretchToFill, true, 10.0f);
		GUI.DrawTexture(new Rect(waveTimerLocation.x + 90,waveTimerLocation.y,waveTimerLocation.width, waveTimerLocation.height), 
		                Numbers[(waveTimer-((waveTimer/10)*10))], ScaleMode.StretchToFill, true, 10.0f);
	}*/
	//115, 415, 120, 25


}