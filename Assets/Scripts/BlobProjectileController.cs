using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobProjectileController : MonoBehaviour {

	public float gravityModifier = 1f;
	public float projectileSpeed; // Magnitude of initial velocity
	public bool isHoming; // Stores whether the projectile is a homing projectile
	public bool isTargetted; // Stores whether the projectile is a targetted projectile (target direction is towards player's location when instantiated)

	private Rigidbody2D rb2d;
	private ContactFilter2D contactFilter;
	private BoxCollider2D boxCollider;
	private Vector2 velocity;
	private Vector2 launcherSpeed; // Speed of the blob when it launched this projectile

	//private SpriteRenderer spriteRenderer;

	void OnEnable () {

		//spriteRenderer = GetComponent<SpriteRenderer> ();
		rb2d = GetComponent<Rigidbody2D>();

		if (isHoming) {
			boxCollider = GetComponent<BoxCollider2D> ();
		}

	}

	void Start () {

		contactFilter.useTriggers = false;
		contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask(gameObject.layer));
		contactFilter.useLayerMask = true;

		if (isTargetted) {
			velocity = launcherSpeed.normalized * projectileSpeed; // Sets the projectile's velocity to be in the direction of the player (blob is also moving in the player's direction)
		} else if (!isHoming) {
			// Gets the initial angle of the projectile from rotation and uses it to find x and y components of velocity. Rotation.z stores sin(angle/2) and rotation.w stores cos(angle/2)
			velocity = new Vector2 (Mathf.Cos (Mathf.Acos(gameObject.transform.rotation.w) * 2), Mathf.Sin (Mathf.Asin(gameObject.transform.rotation.z) * 2)).normalized * projectileSpeed;
			velocity += launcherSpeed; // Adds the speed of the blob when projectile was launched
		}

	}

	void FixedUpdate () {

		rb2d.position += velocity * Time.deltaTime;	
		velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

	}

	public void setLauncherSpeed(Vector2 speed) {
		
		launcherSpeed = speed;

	}

	public void stopProjectile() {
		
		projectileSpeed = 0;

	}

	void OnTriggerStay2D(Collider2D other) {

		// If the projectile is a homing projectile, set velocity to be in the direction of the player
		if (isHoming) {
			if (boxCollider.IsTouching (other) && other.tag == "Player") {
				float deltaX = other.gameObject.GetComponent<Rigidbody2D> ().position.x - rb2d.position.x;
				float deltaY = other.gameObject.GetComponent<Rigidbody2D> ().position.y - rb2d.position.y;
				velocity = new Vector2 (deltaX, deltaY).normalized * projectileSpeed;
			}
		}

	}

}