using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour {

	public float health;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		
		if (other.tag == "Player") {
			PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth> ();
			playerHealth.changeHealth (health);
			Destroy (gameObject);
		}

	}

	void OnTriggerStay2D(Collider2D other) {
		
		if (other.tag == "Player") {
			PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth> ();
			playerHealth.changeHealth (health);
			Destroy (gameObject);
		}

	}

}
