using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHit : MonoBehaviour {

	public float damage;

	private PlayerProjectileController projectileController;

	// Use this for initialization
	void Awake () {
		projectileController = GetComponentInParent<PlayerProjectileController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Shootable")) {
			projectileController.stopProjectile ();
			Destroy (gameObject);
			if (other.tag == "Enemy") {
				EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth> ();
				enemyHealth.changeHealth (-damage);
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Shootable")) {
			projectileController.stopProjectile ();
			Destroy (gameObject);
			if (other.tag == "Enemy") {
				EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth> ();
				enemyHealth.changeHealth (-damage);
			}
		}
	}
}
