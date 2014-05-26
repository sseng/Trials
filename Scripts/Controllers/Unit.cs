using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
	public float moveSpeed = 20.0f;
	public float damage = 10.0f;
	public float coolDownRate = 1.0f;
	public float health = 100.0f;
	public float minHealth = 35.0f;
	public float regenDelay = 5.0f;
	public float regenRate = 1.0f;
	public bool canFlinch = false;
	public float flinchDuration = 0.5f;
	public int killScore = 10;
	[HideInInspector] public float maxHealth;
	float lastDmgTimer = 0;

	public float curHealth {
		get{
			return health;
		}
	}

	//[HideInInspector]
	public int flags = 0;
	// Flags determine the state of the unit.
	public const int DEAD_FLAG = 1 << 0; //is dead.
	public const int STUNNED_FLAG = 1 << 1; //is stunned
	public const int SILENCED_FLAG = 1 << 2; //is silenced
	public const int IMMOBILIZED_FLAG = 1 << 3; //is immobilized
	public const int CC_IMMUNE_FLAG = 1 << 4; //is immune to CC
	public const int DAMGAGE_IMMUNE_FLAG = 1 << 5; //is immune to damage
	public const int MOVE_BLOCK_FLAG = 1 << 6; //is being moved by non-move command

	void Start () {
		if (coolDownRate < 0) coolDownRate = 0;
		maxHealth = health;
	}

	// Update is called once per frame
	void Update () {
		RegenHealth ();
		if (health > maxHealth) health = maxHealth;
		//flinchDuration -= Time.deltaTime;
	}

	void RegenHealth(){
		//If Alive block
		if((flags & DEAD_FLAG) == 0){
			//Regen Health block
			{
				if(lastDmgTimer > 0.0f){
					lastDmgTimer -= Time.deltaTime;
				}
				if(lastDmgTimer <= 0.0f && health < minHealth){
					health += regenRate * Time.deltaTime;
				}
			}
		}
	}
	/// <summary>
	/// Deal damage to the unit.
	/// </summary>
	/// <returns><c>true</c>if the unit dies.<c>false</c> otherwise.</returns>
	/// <param name="dmg">The damage to be done.</param>
	public bool DealDamage(float dmg){
		health -= dmg;
		lastDmgTimer = regenDelay;
		if (canFlinch) {
			Flinch f = GetComponent<Flinch>() as Flinch;
			if (f == null) {
				f = gameObject.AddComponent<Flinch>() as Flinch;
			}
			f.Init (GetComponent<UnitManager>(), this.flinchDuration);
		}
		if (health <= 0)
			return true;
		else
			return false;
	}
}
