using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour {

	public float damage;

	private BlobProjectileController projectileController;
	private CircleCollider2D circleCollider;

	// Use this for initialization
	void Awake () {
		projectileController = GetComponentInParent<BlobProjectileController> ();
		circleCollider = GetComponent<CircleCollider2D> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D other) {
		if (circleCollider.IsTouching (other)) {
			if (other.tag == "Player") {
				projectileController.stopProjectile ();
				Destroy (gameObject);
				PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth> ();
				playerHealth.changeHealth (-damage);
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (circleCollider.IsTouching (other)) {
			if (other.tag == "Player") {
				projectileController.stopProjectile ();
				Destroy (gameObject);
				PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth> ();
				playerHealth.changeHealth (-damage);
			}
		}
	}
}
