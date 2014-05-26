
using UnityEngine;
using System.Collections;
using Pathfinding;

public class AIController : Controller
{
	private Seeker seeker;
	//The calculated path
	public Path path;
	//The end of the path
	[HideInInspector]
	public GameObject
		pathTarget;
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	//The Rate that the computer refreshes it's path
	public float researchRate = 1.0f;
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
		searchTimer = researchRate;
		seeker = GetComponent<Seeker> ();
		pathTarget = GameObject.FindGameObjectWithTag ("Player");
		//Start a new path to the targetPosition, return the result to the OnPathComplete function
		seeker.StartPath (transform.position, pathTarget.transform.position, OnPathComplete);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (pathTarget != null) {
			if (searchTimer >= 0) searchTimer -= Time.deltaTime;
			Ray sight = new Ray (transform.position + new Vector3 (0, transform.localScale.y * GetComponent<CharacterController> ().center.y), pathTarget.transform.position - transform.position);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (sight, out hit, Mathf.Infinity, Utilities.ENEMY_LAYER | Utilities.PLAYER_LAYER | Utilities.OBSTACLE_LAYER)) {
				target = null;
				if (hit.collider.gameObject == pathTarget) {
					foreach (Ability a in UM.getAbilities()) {
						if (hit.distance <= a.range-5) {
							target = pathTarget;
							if (a.CoolDown <= 0) {
								UM.AddAction (ActionType.ABILITY, target.transform.position, a);
							}
						}
					}
				}
			}

			if (target == null) {
				if (searchTimer <= 0.0f) {
					seeker.StartPath (transform.position, pathTarget.transform.position, OnPathComplete);
					searchTimer = researchRate;
				}
				if (path != null) {
					//Check if we are close enough to the next waypoint
					//If we are, proceed to follow the next waypoint
					if (Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]) < nextWaypointDistance &&
					    currentWaypoint < path.vectorPath.Count - 1) {
						currentWaypoint++;
					}
					UM.AddAction (ActionType.MOVE, path.vectorPath [currentWaypoint]);
				}
				else Debug.Log ("No Path");
			}
		}
	}
}