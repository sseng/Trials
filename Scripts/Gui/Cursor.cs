using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cursor : MonoBehaviour {
    public bool DisablePointer;
	public GameObject movementParticle;
	public GameObject cursorParticle;
	public Texture2D cursorImage;
	public Vector2 cursorImageSize;
	public Vector2 cursorImageOffset;
	public Vector2 FlamePosOffset;
	public int maxRings = 3;

	private GameObject cursor;
	private Vector3 worldPosition;
	private Vector3 screenPosition;

	private List<GameObject> rings = new List<GameObject>();
	private int moveableLayer = 1 << 10;
	private Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);


	void Start(){
		//cursorTexture = Instantiate(cursorTexture) as GUITexture;
		cursor = (GameObject)Instantiate(cursorParticle);
		//movementParticle.renderer.material.renderQueue = 2900;
	}
	void Update () {
		disablePointer();
		//flame particle on the cursor torch
		Vector3 flamePos = Input.mousePosition;
		flamePos.x += FlamePosOffset.x;
		flamePos.y += FlamePosOffset.y;
		Ray ray = Camera.main.ScreenPointToRay(flamePos);
		cursor.transform.position = ray.GetPoint(6);

		//spawn ring movement indicators
		if(Input.GetMouseButtonUp(0) && hitPoint() != invalidPosition){
			spawnRing(movementParticle, hitPoint(), maxRings);
		}

	}
	void OnGUI(){
		Event e = Event.current;
		//Draws Cursor texture
		GUI.DrawTexture(new Rect(e.mousePosition.x + cursorImageOffset.x, e.mousePosition.y+cursorImageOffset.y, cursorImageSize.x, cursorImageSize.y), cursorImage);
	}

	GameObject spawnRing(GameObject particle, Vector3 location, int maxRingSpawns){
		Vector3 ringLocation = location;
		Quaternion ringRotation = Quaternion.AngleAxis(90, Vector3.right);
		GameObject ring = (GameObject)Instantiate (movementParticle, ringLocation, ringRotation);
		rings.Add (ring);
		while (rings.Count > maxRingSpawns) {
			if(rings[0] != null){
				Destroy(rings[0].gameObject);
			}
			rings.RemoveAt(0);
		}
		return ring;
	}

	void disablePointer(){
        if(DisablePointer){
            Screen.showCursor = false;
        }else{
            Screen.showCursor = true;   
        }
    }

	Vector3 hitPoint(){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();

		if(Physics.Raycast(ray, out hit, Mathf.Infinity, moveableLayer))return hit.point;
		else{
			return invalidPosition;
		}

	}
}
