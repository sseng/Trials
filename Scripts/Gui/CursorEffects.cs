using UnityEngine;
using System.Collections;

public class CursorEffects : MonoBehaviour {
	public GUIText onScreenText;

	private GUIText guiText;
    private float rise;
    private int moveableLayer = 1 << 8;
    private Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
	
    void Update(){
        if(Input.GetMouseButtonUp(0) && hitPoint() != invalidPosition){
            displayDamage("test!!", hitPoint());
        }      
    }
    
    public Vector3 hitPoint(){
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit hit = new RaycastHit ();
        if (Physics.Raycast (ray, out hit, Mathf.Infinity, moveableLayer)) {
            return hit.point;
        } 
        else {
            return invalidPosition;
        }
    }
    
    void displayDamage(string message, Vector3 location){
       
        Vector3 textLocation = Camera.main.WorldToScreenPoint(location);
        textLocation.x /= Screen.width;
        textLocation.y /= Screen.height;       
        guiText = Instantiate(onScreenText, textLocation, Quaternion.identity) as GUIText;
        onScreenText.text = message;
    }
}
