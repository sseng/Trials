using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ActionType {MOVE, ABILITY, STATUS};

[RequireComponent(typeof(Unit))]
[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class UnitManager : MonoBehaviour {
	public string unitName;
	float minMoveDistance = 3.5f;
	float busy = 0.0f;
	Unit unit;
	CharacterController charController;
	Animator animator;
	Ability[] abilities;
	string[] omegaTriggers = {"Slash", "Spin", "Charge"};
	string[] minotaurTriggers = {"Slash", "Throw", "Flinch"};
	string[] harpyTriggers = {"Slash", "Scream", "Flinch"};
	string[] cerberusTriggers = {"Slash", "Fire", "Pounce"};
		
	class Action
	{
		public Action(ActionType t, Vector3 p, Ability d = null){
			type = t;
			mousePos = p;
			data = d;
		}
		public ActionType type;
		public Vector3 mousePos;
		public Ability data;
	} 

	Action nextAction;

	// Use for GetComponent Calls
	void Awake() {
		unit = GetComponent<Unit> ();
		charController = GetComponent<CharacterController> ();
		animator = GetComponent<Animator> ();
		abilities = GetComponents<Ability> ();		
		if (gameObject.tag != "Player" && gameObject.tag != "Unit") {
			gameObject.tag = "Unit";
			if (gameObject.layer != 10) {
				gameObject.layer = 10;
			}
		} else if (gameObject.tag == "Player" && gameObject.layer != 11) {
			gameObject.layer = 11;
		}
	}

	// Use this for initialization
	void Start () {

	}

	void Update() {
		if (nextAction != null){
			if (nextAction.type == ActionType.ABILITY) {
				Execute ();
			}
		}
		if (busy > 0) {
			busy -= Time.deltaTime;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (nextAction != null) {
			if (nextAction.type == ActionType.MOVE) {
				if ((unit.flags & (Unit.DEAD_FLAG | Unit.IMMOBILIZED_FLAG | Unit.STUNNED_FLAG)) == 0) {
					if (nextAction.type == ActionType.MOVE) {
						switch(unitName){
						case "Omega":
							foreach (string s in omegaTriggers) {					
								animator.ResetTrigger (s);
							}
							break;
						case "Minotaur":
							foreach (string s in minotaurTriggers) {					
								animator.ResetTrigger (s);
							}
							break;
						case "Harpy":
							foreach (string s in harpyTriggers) {					
								animator.ResetTrigger (s);
							}
							break;
						case "Cerberus":
							foreach (string s in cerberusTriggers) {					
								animator.ResetTrigger (s);
							}
							break;
						}
					}
					if ((unit.flags & Unit.MOVE_BLOCK_FLAG) == 0) {
						RotateTowards (nextAction.mousePos);
					}
					MovePlayer (nextAction.mousePos);
				}
			}
		}
	}

	/// <summary>
	/// Rotates the towards the destination.
	/// </summary>
	/// <param name="destination">Destination.</param>
	private void RotateTowards(Vector3 destination){
		Quaternion targetRotation = Quaternion.LookRotation (destination - transform.position);
		transform.rotation = targetRotation;
	}
	
	private void MovePlayer(Vector3 destination){
		float destinationDistance = Vector3.Distance (destination, transform.position);
		if (destinationDistance >= minMoveDistance) {
			charController.SimpleMove ((destination - transform.position).normalized * unit.moveSpeed);
			AddAction (ActionType.MOVE, destination);
			if (unitName == "Omega" || unitName == "Minotaur" || unitName == "Cerberus") {
				animator.SetBool ("Moving", true);
			}
		} else {
			if (unitName == "Omega" || unitName == "Minotaur" || unitName == "Cerberus") {
				animator.SetBool ("Moving", false);
			}
		}
	}

	public void SetAnimation(string name, bool state)
	{
		animator.SetBool (name, state);
	}

	public void AddAction (ActionType name, Vector3 mousePos, Ability data = null)
	{
		nextAction = new Action (name, mousePos, data);
		Execute();
	}

	public void ChangeDirection (Vector3 mousePos)
	{
		if (nextAction != null)
		{
			if (nextAction.type == ActionType.MOVE) nextAction.mousePos = mousePos;
		}
	}

	public void AddDamage (UnitManager source, float d)
	{
		if (unit.DealDamage (d)) {
			source.AddScore (unit.killScore);
			animator.SetTrigger ("Dead");
			collider.enabled = false;
			AddFlags (Unit.DEAD_FLAG);
			foreach (Buff b in GetComponents<Buff>())
			{
				Destroy (b);
			}
			Destroy (this.gameObject, 5.0f);
		}
		if ((unit.flags & Unit.STUNNED_FLAG) == Unit.STUNNED_FLAG && unitName != "Omega") {
			animator.SetTrigger ("Flinch");
		}

	}

	public void ChangeStatsFlat (float speed,
	                         	 float damage,
	                         	 float cdr,
	                         	 float minHealth,
	                         	 float regenDelay,
	                         	 float regenRate){
		unit.moveSpeed += speed;
		unit.damage += damage;
		unit.coolDownRate += cdr;
		unit.minHealth += minHealth;
		unit.regenDelay += regenDelay;
		unit.regenRate += regenRate;
	}

	public void ChangeStatsPercent (float speed,
	                            	float damage,
	                            	float cdr,
	                             	float minHealth,
	                             	float regenDelay,
	                             	float regenRate){
		unit.moveSpeed *= speed;
		unit.damage *= damage;
		unit.coolDownRate *= cdr;
		unit.minHealth *= minHealth;
		unit.regenDelay *= regenDelay;
		unit.regenRate *= regenRate;
	}

	public Unit getUnit ()
	{
		return unit;
	}

	public void AddFlags (int flags){
		unit.flags = unit.flags | flags;
		if ((flags & (Unit.DEAD_FLAG | Unit.SILENCED_FLAG | Unit.STUNNED_FLAG)) != 0) {
			foreach (Ability a in abilities){
				a.StopCoroutine ("Execute");
			}
		}
	}
	public void RemoveFlags (int flags){
		unit.flags = unit.flags & (~flags);
	}

	void AddScore (int killScore)
	{
		WaveGenerator.enemyCount--;
		UI_Script.addScore(killScore);
	}

	public List<float> GetStats ()
	{
		List<float> temp = new List<float>();
		temp.Add (unit.moveSpeed);
		temp.Add (unit.damage);
		temp.Add (unit.coolDownRate);
		temp.Add (unit.minHealth);
		temp.Add (unit.regenDelay);
		temp.Add (unit.regenRate);
		return temp;
	}

	public Ability[] getAbilities ()
	{
		return abilities;
	}

	public AnimatorStateInfo GetAnimatorInfo()
	{
		return animator.GetCurrentAnimatorStateInfo (0);
	}

	void Execute ()
	{
		if (nextAction.type == ActionType.ABILITY){

			if ((unit.flags & (Unit.DEAD_FLAG | Unit.SILENCED_FLAG | Unit.STUNNED_FLAG)) == 0) {
				if (unitName != "Harpy") animator.SetBool ("Moving", false);
				switch (unitName){
				case "Omega":
					foreach (string s in omegaTriggers) {
						if (s == nextAction.data.abilityName)
							animator.SetTrigger (s);
						else
							animator.ResetTrigger (s);
					}
					break;
				case "Minotaur":
					foreach (string s in minotaurTriggers) {
						if (s == nextAction.data.abilityName)
							animator.SetTrigger (s);
						else
							animator.ResetTrigger (s);
					}
					break;
				case "Harpy":
					foreach (string s in harpyTriggers) {
						if (s == nextAction.data.abilityName)
							animator.SetTrigger (s);
						else
							animator.ResetTrigger (s);
					}
					break;
				case "Cerberus":
					foreach (string s in cerberusTriggers) {
						if (s == nextAction.data.abilityName)
							animator.SetTrigger (s);
						else
							animator.ResetTrigger (s);
					}
					break;
				}

				if (busy <= 0) {
					RotateTowards (nextAction.mousePos);
					StartCoroutine (nextAction.data.Execute (unit.damage, unit.coolDownRate, nextAction.mousePos, this));
					busy = nextAction.data.animationTime;
					nextAction = null;
				}
			}
		}
	}

	/*void SortNextAction()
	{
		while (actionQueue.Count > 0)
		{
			Action temp = (Action)actionQueue.Dequeue ();
			if (temp.name != null) {
				switch (temp.name){
				case Action.NEW_MOVE:
					nextAction = temp;
					break;
				case Action.ABILITY:
					if(nextAction.name != Action.NEW_MOVE){
						nextAction = temp;
					}
					break;
				case Action.MOVE:
					if (nextAction.name != Action.NEW_MOVE && nextAction.name != Action.ABILITY){
						nextAction = temp;
					}
					break;
				default:
					break;
				}
			}
		}
		if (nextAction.name != null)
			DispatchAction (nextAction);
	}*/

	/*void DispatchAction (Action nextAction)
	{
		if (nextAction.name == Action.MOVE || nextAction.name == Action.NEW_MOVE){
			bool moveable;
			if (animator.IsInTransition (0)){
				moveable = animator.GetNextAnimatorStateInfo(0).IsTag("Moveable");
			}else moveable = animator.GetCurrentAnimatorStateInfo(0).IsTag("Moveable");
			if ((unit.flags & (Unit.DEAD_FLAG|Unit.IMMOBILIZED_FLAG|Unit.STUNNED_FLAG|Unit.MOVE_BLOCK_FLAG)) == 0
			     &&  moveable){
				if (nextAction.name == Action.NEW_MOVE){
					foreach (string s in triggers){					
						animator.ResetTrigger(s);
					}
				}
				RotateTowards (nextAction.mousePos);
			    MovePlayer (nextAction.mousePos);
			}
		} else if (nextAction.name == Action.ABILITY) {
			if ((unit.flags & (Unit.DEAD_FLAG|Unit.SILENCED_FLAG|Unit.STUNNED_FLAG)) == 0){
				Ability temp = (Ability)nextAction.data;
				nextAbilityName = temp.abilityName;
				abilityMousePos = nextAction.mousePos;
				temp.mousePos = abilityMousePos;
				animator.SetBool("Moving", false);
				foreach (string s in triggers){
					if (s == temp.abilityName) animator.SetTrigger(s);
					else animator.ResetTrigger(s);
				}
			}
		}
	}*/
}
