using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveGenerator : MonoBehaviour {
	[System.Serializable]
	public class wave{
		public List<GameObject> spawnPoints;
		public List<GameObject> Enemy;
	}
	public List<wave> Waves;
	[HideInInspector]
	public int currentWave;

	[HideInInspector]
	static public int enemyCount;
	[HideInInspector]
	public float Wavetimer;
	public float WaveTimelimit;

	// Use this for initialization
	void Start () {
		Wavetimer = WaveTimelimit;
	}
	
	// Update is called once per frame
	void Update () {
		float c = Time.deltaTime;
		Wavetimer = Wavetimer - c;

		if (enemyCount == 0) {
			if(currentWave< Waves.Count && Wavetimer>5)
			{
				Wavetimer=5;
			}
		}
		if(Wavetimer<=0){
			if(currentWave< Waves.Count)
			{
				SpawnEnemies(Waves, currentWave);
				currentWave++;
				if (currentWave >= Waves.Count) currentWave = Waves.Count - 1;
				Wavetimer=WaveTimelimit;
			}
		}

	}

	void SpawnEnemies(List<wave> Waves, int currentWave){
		for (int i=0; i<Waves[currentWave].Enemy.Count; i++)
		{
			Instantiate(Waves[currentWave].Enemy[i],Waves[currentWave].spawnPoints[i].transform.position, Quaternion.identity);
			enemyCount++;
		}

	}

}
