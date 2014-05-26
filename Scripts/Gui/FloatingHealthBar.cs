using UnityEngine;
using System.Collections;

public class FloatingHealthBar : MonoBehaviour {

	[System.Serializable]
	public class HealthBar{
		public bool useDefault = true;
		public Vector2 position;
		public float width;
		public float height;
	}

	public HealthBar health;

	private Transform meterLocation;
	private GameObject _healthbar;
	private float curHP, maxHP;
	private Unit unit;
	private bool meterEnabled;
	private float sw, sh;
	 
	void Start () {
		unit = this.gameObject.GetComponent<Unit>();
		GameObject go = (GameObject)Resources.Load("Gui/RedMeter");
		if(!_healthbar){
			_healthbar = (GameObject)Instantiate(go);
			_healthbar.transform.parent = this.gameObject.transform;
		}
		if(health.useDefault){
			health.position.x  = -10f;
			health.position.y = 25f;
			health.width = 100;
			health.height = 12;
		}
	}

	void Update () {
		curHP = unit.curHealth;
		maxHP = unit.maxHealth;

		sw = Screen.width * 1/1024f;
		sh = Screen.height * 1/768f;

		if(_healthbar != null){
			SetBarPosition(health.position.x, health.position.y);
			AdjustMeter(0,0, health.width*sw, health.height*sh, curHP, maxHP);
			ToggleMeter();
			EnableMeter();
		}
	}

	void SetBarPosition(float x, float y){
		Vector3 meterPos = this.gameObject.transform.position;
		meterPos.x += x;
		meterPos.y += y;
		Vector3 meterPosition = Camera.main.WorldToViewportPoint(meterPos);
		_healthbar.transform.position = meterPosition;
	}

	void AdjustMeter(float x, float y, float w, float h, float curhp, float maxhp){
		float hp = (curhp/maxhp) * w;
		_healthbar.guiTexture.pixelInset = new Rect(0, 0, hp, h);
	}

	void ToggleMeter(){
		if(curHP <= 0){
			Destroy(_healthbar.gameObject);
		}
		if(curHP < maxHP){
			meterEnabled = true;
		}
		else{
			meterEnabled = false;
		}
	}

	void EnableMeter(){
		if (meterEnabled == true){
			_healthbar.gameObject.SetActive(true);
		}
		if(meterEnabled == false){
			_healthbar.gameObject.SetActive(false);
		}
	}
}
