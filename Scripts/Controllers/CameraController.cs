using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public GameObject player;
	public float maxX = 20.0f, minX = -20.0f,
		maxZ = 20.0f, minZ = -20.0f;
	private Vector3 relativePosition;

	// Use this for initialization
	void Start ()
	{
		if (player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
		relativePosition = transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void LateUpdate ()
	{
		if (player != null) {
			transform.position = relativePosition + player.transform.position;
			Ray bottomLeft = camera.ScreenPointToRay (new Vector3 (0, 0, 0));
			Ray topRight = camera.ScreenPointToRay (new Vector3 (this.camera.pixelWidth, this.camera.pixelHeight, 0));
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (bottomLeft, out hit)) {
				if (hit.point.x < this.minX) {
					Vector3 tempPos = transform.position;
					tempPos.x += (this.minX - hit.point.x);
					transform.position = tempPos;
					
				}
				if (hit.point.z < this.minZ) {
					Vector3 tempPos = transform.position;
					tempPos.z += (this.minZ - hit.point.z);
					transform.position = tempPos;
				}
			}
			if (Physics.Raycast (topRight, out hit)) {
				if (hit.point.x > this.maxX) {
					Vector3 tempPos = transform.position;
					tempPos.x += (this.maxX - hit.point.x);
					transform.position = tempPos;
				}
				if (hit.point.z > this.maxZ) {
					Vector3 tempPos = transform.position;
					tempPos.z += (this.maxZ - hit.point.z);
					transform.position = tempPos;
				}
			}
		}
	}
}
