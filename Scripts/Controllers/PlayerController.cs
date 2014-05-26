using UnityEngine;
using System.Collections;
using Pathfinding;

public class PlayerController : Controller {
	private Seeker seeker;
	//The calculated path
	public Path path;
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 10;
	float searchTimer;
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;
	
	public void OnPathComplete (Path p)
	{
		Debug.Log ("Yey, we got a path back. Did it have an error? " + p.error);
		if (!p.error) {
			path = p;
			//Reset the waypoint counter
			currentWaypoint = 0;
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		seeker = GetComponent<Seeker> ();
	}
	void Update () {
		if (searchTimer >= 0) searchTimer -= Time.deltaTime;
		mouseClick ();
		CheckAbilities(); // Check to see if any of the abilities keys have been used
		if (path != null) {
			//Check if we are close enough to the next waypoint
			//If we are, proceed to follow the next waypoint
			if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance &&
			    currentWaypoint < path.vectorPath.Count - 1) {
				currentWaypoint++;
			}
			UM.AddAction (ActionType.MOVE, path.vectorPath [currentWaypoint]);
			if (currentWaypoint == path.vectorPath.Count - 1){
				path = null;
			}
		}
	}

	void mouseClick(){
		if (Input.GetMouseButtonDown (0)) {
			//Move
			Vector3 mousePos = Utilities.GetMousePosition ();
			if (mousePos.magnitude < Mathf.Infinity)
				seeker.StartPath (transform.position, mousePos, OnPathComplete);
		}
		else if (Input.GetMouseButton (0)) {
			//Change Move Target
			Vector3 mousePos = Utilities.GetMousePosition ();
			if (mousePos.magnitude < Mathf.Infinity && searchTimer <= 0.0f) {
				seeker.StartPath (transform.position, mousePos, OnPathComplete);
			}
		}
	}

	void CheckAbilities(){
		foreach (Ability a in UM.getAbilities()) {
			if (Input.GetKeyDown (a.key)){
				if(a.CoolDown <= 0){
					//Use Ability
					UM.AddAction (ActionType.ABILITY, Utilities.GetMousePosition (), a);
					path = null;
				}
			}
		}
	}
}
