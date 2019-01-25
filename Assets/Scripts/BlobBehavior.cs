using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobBehavior: MonoBehaviour {

	public float enemySpeed;
	public float rangeModifier; // How much the dimensions of the enemy's vision range will be multiplied by during chase
	public float healthRegeneration; // Amount of health regeneration per tick
	public float healthRegenerationRate; // Rate at which health regenerates
	private float nextHealthRegeneration = 0f; // Stores time of next health regen tick
	private bool inCombat = false;

	private Vector2 idlePosition;
	private Rigidbody2D rb2d;
	private BoxCollider2D boxCollider;
	private Animator animator;
	private EnemyHealth health;

	// Use this for initialization
	void Start () {
		
		rb2d = GetComponentInParent<Rigidbody2D> (); // Gets parent Rigidbody2D
		boxCollider = GetComponent<BoxCollider2D> (); // Gets BoxCollider2D
		animator = GetComponentInParent<Animator> (); // Gets Animator
		health = GetComponentInParent<EnemyHealth> (); // Gets heatlh of enemy
		idlePosition = new Vector2 (rb2d.position.x, rb2d.position.y); // Sets idlePosition to the starting position of the parent object

	}
	
	// Update is called once per frame
	void Update () {

		// If not chasing player, return to idle position
		if (!inCombat) {
			float deltaX = idlePosition.x - rb2d.position.x;
			float deltaY = idlePosition.y - rb2d.position.y;
			Vector2 targetMovement = new Vector2 (deltaX, deltaY); // Vector containing distance between idle position and current position

			// Checks if the distance to idle position is less than enemySpeed, if it is then just move to idle position, if not then move a distance equal to enemySpeed
			rb2d.position = rb2d.position + (targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement);
		
			if (rb2d.position.x == idlePosition.x && rb2d.position.y == idlePosition.y) {
				if (Time.time > nextHealthRegeneration) {
					health.changeHealth (healthRegeneration);
					nextHealthRegeneration = Time.time + healthRegenerationRate;
				}
			}
		}

		animator.SetBool ("isInCombat", inCombat); // Returns whether enemy is chasing
		animator.SetBool ("atIdlePosition", rb2d.position.x == idlePosition.x && rb2d.position.y == idlePosition.y); // Returns whether enemy is at its idle position

	}

	// Chase player if player enters chase range
	void OnTriggerEnter2D(Collider2D other) {
		
		if (other.tag == "Player") {
			inCombat = true;
			boxCollider.size = new Vector2 (boxCollider.size.x * rangeModifier, boxCollider.size.y * rangeModifier);
		}

	}

	// Chase code
	void OnTriggerStay2D(Collider2D other) {
		
		if (other.tag == "Player") {
			float deltaX = other.gameObject.GetComponent<Rigidbody2D> ().position.x - rb2d.position.x;
			float deltaY = other.gameObject.GetComponent<Rigidbody2D> ().position.y - rb2d.position.y;
			Vector2 targetMovement = new Vector2 (deltaX, deltaY); // Vector containing distance between player and this enemy

			// Checks if distance to player is less than enemySpeed, if it is then just move to player, if not then move a distance equal to enemySpeed
			rb2d.position = rb2d.position + (targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement);
		}

	}

	// Stop chasing if player ever leaves range
	void OnTriggerExit2D(Collider2D other) {
		
		if (other.tag == "Player") {
			inCombat = false;
			boxCollider.size = new Vector2 (boxCollider.size.x / rangeModifier, boxCollider.size.y / rangeModifier);
		}

	}

}
