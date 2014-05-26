using UnityEngine;
using System.Collections;

public static class Utilities{
	public const int OBSTACLE_LAYER = 1 << 8;
	public const int ENEMY_LAYER = 1 << 9;
	public const int GROUND_LAYER = 1 << 10;
	public const int PLAYER_LAYER = 1 << 11;
	public static Vector3 GAME_HEIGHT = new Vector3 (0, 6.67f);

	/// <summary>
	/// Gets the mouse position in the game plane.
	/// </summary>
	/// <returns>The mouse position in the game plane.</returns>
	public static Vector3 GetMousePosition(){
		Vector3 position = new Vector3();
		Ray target = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (target, out hit, Mathf.Infinity, ENEMY_LAYER|GROUND_LAYER)){
			if (hit.collider.gameObject.layer == 9) {
				position = hit.collider.transform.position;
			}
			else if (hit.collider.gameObject.layer == 10) {
				position = hit.point;
			}
			return position;
		}
		else return new Vector3(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity);
	}
}
