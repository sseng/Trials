using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Controller : MonoBehaviour {
	protected UnitManager UM;
	[HideInInspector]	public GameObject target;

	// Use to initilize references
	protected void Awake () {
		UM = GetComponent<UnitManager> ();
	}
}
