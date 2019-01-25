using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileController : PhysicsObject {

	public float projectileSpeed;
	//private SpriteRenderer spriteRenderer;

	void Awake () {

		//spriteRenderer = GetComponent<SpriteRenderer> ();
		rb2d = GetComponent<Rigidbody2D>();

	}

	// Actually just moves the bullet directly
	protected override void ComputeVelocity () {
		
		if (transform.rotation.z == 0) {
			rb2d.position = rb2d.position + Vector2.right * projectileSpeed;
		} else {
			rb2d.position = rb2d.position + Vector2.left * projectileSpeed;
		}

	}

	public void stopProjectile() {
		projectileSpeed = 0;
	}

}
