using UnityEngine;
using System.Collections;

public class MoveText : MonoBehaviour {
    public float riseSpeed = 1.0f;
	[Range(0.1f, 5f)] public float duration;
    private float timer = 0;

	void Update () {
      
        timer += Time.deltaTime;
        this.transform.position += Vector3.up * Time.deltaTime * 0.1f * riseSpeed;

        if(timer > duration){
            Destroy(this.gameObject);
        }
	}
}
