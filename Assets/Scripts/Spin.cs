using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
	public float Speed = 5;

	void Update () {
		gameObject.transform.Rotate(
			new Vector3(
				Random.value * Speed,
				Random.value * Speed,
				Random.value * Speed
			)
		);
	}
}
