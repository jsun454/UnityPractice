using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

	public float jumpTakeOffSpeed = 7;
	public float maxSpeed = 7;

	public Transform projectileLauncher;
	public GameObject projectile;
	public float rateOfFire = 0.5f;
	private float nextFire = 0f;

	private SpriteRenderer spriteRenderer;
	//private Animator animator;

	// Use this for initialization
	void Awake () {

		spriteRenderer = GetComponent<SpriteRenderer> ();
		//animator = GetComponent<Animator> ();

	}

	void Update () {
		ComputeVelocity ();
		if (Input.GetAxisRaw ("Fire1") > 0) {
			launchProjectile ();
		}
	}

	protected override void ComputeVelocity() {

		Vector2 move = Vector2.zero;

		move.x = Input.GetAxis ("Horizontal");

		if (Input.GetButtonDown ("Jump") && grounded) {
			velocity.y = jumpTakeOffSpeed;
		} else if (Input.GetButtonUp ("Jump")) {
			if (velocity.y > 0) {
				velocity.y = velocity.y * .5f;
			}
		}

		bool flipSprite = spriteRenderer.flipX ? move.x < -.01f : move.x > .01f;
		if (flipSprite) {
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}

		//animator.SetBool ("grounded", grounded);
		//animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);

		targetVelocity = move * maxSpeed;

	}

	void launchProjectile () {
		if (Time.time > nextFire) {
			nextFire = Time.time + rateOfFire;

			if (spriteRenderer.flipX) {
				Instantiate (projectile, projectileLauncher.position, Quaternion.Euler (Vector3.zero));
			} else {
				Instantiate (projectile, projectileLauncher.position, Quaternion.Euler (new Vector3 (0, 0, 180f)));
			}
		}
	}

}
