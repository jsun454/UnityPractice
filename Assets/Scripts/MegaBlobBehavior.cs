using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaBlobBehavior: MonoBehaviour {

	public float enemySpeed; // Enemy speed
	public float rageSpeed; // Enemy speed during rage (phase four)
	public float rangeModifier; // How much the dimensions of the enemy's vision range will be multiplied by during chase
	public float healthRegeneration; // Amount of health regeneration per tick
	public float healthRegenerationRate; // Rate at which health regenerates
	public float rateOfSpreadFire; // Rate at which enemy attacks (actually just stores time between fires)
	public float rateOfHomingFire; // Rate at which enemy shoots homing attacks (actually just stores time between fires)
	public float rateOfTargettedFire; // Rate at which enemy shoots targetted attacks (actually just stores time between fires)
	private float nextHealthRegeneration = 0f; // Stores time of next health regen tick
	private float nextSpreadFire = 0f; // Stores time of next attack
	private float nextHomingFire = 0f; // Stores time of next homing attack
	private float nextTargettedFire = 0f; // Stores time of next targetted fire

	private Vector2 idlePosition;
	private Vector2 targetMovement;
	private Rigidbody2D rb2d;
	private BoxCollider2D boxCollider;
	private Animator animator;
	private EnemyHealth health;
	public GameObject normalProjectile; // Regular projectile (used in phase 2 and 3)
	public GameObject homingProjectile; // Homing projectile (used in phase 3)
	public GameObject targettedProjectile; // Targetted projectile (used in phase 4)

	private const int phaseIdle = 0;
	private const int phaseOne = 1;
	private const int phaseTwo = 2;
	private const int phaseThree = 3;
	private const int phaseFour = 4;

	private int currentPhase;

	// Use this for initialization
	void Start () {

		rb2d = GetComponentInParent<Rigidbody2D> (); // Gets parent Rigidbody2D
		boxCollider = GetComponent<BoxCollider2D> (); // Gets BoxCollider2D
		animator = GetComponentInParent<Animator> (); // Gets Animator
		health = GetComponentInParent<EnemyHealth> (); // Gets heatlh of enemy
		idlePosition = new Vector2 (rb2d.position.x, rb2d.position.y); // Sets idlePosition to the starting position of the parent object
		currentPhase = phaseIdle;

	}

	// Update is called once per frame
	void FixedUpdate () {

		// Sets current phase based on remaining health of blob
		if (currentPhase != phaseIdle) {
			if ((float)health.getCurrentHealth() <= health.maxHealth * 1f && (float)health.getCurrentHealth() > health.maxHealth * 0.75f) {
				currentPhase = phaseOne;
			} else if ((float)health.getCurrentHealth() <= health.maxHealth * 0.75f && (float)health.getCurrentHealth() > health.maxHealth * 0.5f) {
				currentPhase = phaseTwo;
			} else if ((float)health.getCurrentHealth() <= health.maxHealth * 0.5f && (float)health.getCurrentHealth() > health.maxHealth * 0.25f) {
				currentPhase = phaseThree;
			} else if ((float)health.getCurrentHealth() <= health.maxHealth * 0.25f && (float)health.getCurrentHealth() > 0f) {
				currentPhase = phaseFour;
			}
		}

		// Does different behaviors depending on phase
		switch (currentPhase) {
		case phaseIdle: // If not in combat, return to idle position
			float deltaX = idlePosition.x - rb2d.position.x;
			float deltaY = idlePosition.y - rb2d.position.y;
			targetMovement = new Vector2 (deltaX, deltaY); // Vector containing distance between idle position and current position

				// Checks if the distance to idle position is less than enemySpeed, if it is then just move to idle position, if not then move a distance equal to enemySpeed
			rb2d.position = rb2d.position + (targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement);

			if (rb2d.position.x == idlePosition.x && rb2d.position.y == idlePosition.y) {
				if (Time.time > nextHealthRegeneration) {
					health.changeHealth (healthRegeneration);
					nextHealthRegeneration = Time.time + healthRegenerationRate;
				}
			}
			break;
		case phaseOne : // Enter phase one, which is just chasing (all taken care of in OnTriggerStay2D)
			break;
		case phaseTwo : // Enter phase two, during which the blob shoots out slow travelling blob projectiles affected by gravity
			if (Time.time > nextSpreadFire) {
				nextSpreadFire = Time.time + rateOfSpreadFire;
				fireProjectileSpread ();
			}
			break;
		case phaseThree: // Enter phase three, during which the blob also starts shooting out faster blob projectiles unaffected by gravity
			//TODO: Write code for phase three projectile
			if (Time.time > nextSpreadFire) {
				nextSpreadFire = Time.time + rateOfSpreadFire;
				fireProjectileSpread ();
			}

			if (Time.time > nextHomingFire) {
				nextHomingFire = Time.time + rateOfHomingFire;
				fireHomingProjectile ();
			}
			break;
		case phaseFour: // Enter phase four, during which the blob starts charging faster than before, and shoots out the blobs from phase three with faster velocity and frequency
			enemySpeed = rageSpeed;
			if (Time.time > nextTargettedFire) {
				nextTargettedFire = Time.time + rateOfTargettedFire;
				fireTargettedProjectile ();
			}
			break;
		}

		animator.SetBool ("isInCombat", currentPhase != phaseIdle); // Returns whether enemy is chasing
		animator.SetBool ("atIdlePosition", rb2d.position.x == idlePosition.x && rb2d.position.y == idlePosition.y); // Returns whether enemy is at its idle position

	}

	// Chase player if player enters chase range
	void OnTriggerEnter2D(Collider2D other) {

		if (other.tag == "Player") {
			currentPhase = phaseOne;
			boxCollider.size = new Vector2 (boxCollider.size.x * rangeModifier, boxCollider.size.y * rangeModifier);
		}

	}

	// Chasing and charging code
	void OnTriggerStay2D(Collider2D other) {

		if (other.tag == "Player") {
			float deltaX = other.gameObject.GetComponent<Rigidbody2D> ().position.x - rb2d.position.x;
			float deltaY = other.gameObject.GetComponent<Rigidbody2D> ().position.y - rb2d.position.y;
			targetMovement = new Vector2 (deltaX, deltaY); // Vector containing distance between player and this enemy

			// Checks if distance to player is less than enemySpeed, if it is then just move to player, if not then move a distance equal to enemySpeed
			rb2d.position = rb2d.position + (targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement);
		}

	}

	// Stop chasing if player ever leaves range (shouldn't happen because range increases by factor of 100)
	void OnTriggerExit2D(Collider2D other) {

		if (other.tag == "Player") {
			currentPhase = phaseIdle;
			boxCollider.size = new Vector2 (boxCollider.size.x / rangeModifier, boxCollider.size.y / rangeModifier);
		}

	}

	private void fireProjectileSpread() {
		
		// Fires 5 projectiles simultaneously in an upwards arc
		GameObject projectileA = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 45f));
		GameObject projectileB = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 75f));
		GameObject projectileC = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 90f));
		GameObject projectileD = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 105f));
		GameObject projectileE = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 135f));

		// Adds blob's current speed to projectile's starting speed so the projectiles follow the blob's movement's at the start (somewhat)
		projectileA.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileB.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileC.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileD.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileE.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);

	}

	private void fireHomingProjectile() {
		
		Instantiate (homingProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 0f));

	}

	private void fireTargettedProjectile() {

		GameObject projectileF = Instantiate (targettedProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 0f));
		projectileF.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);

	}

	private void fireHeavyProjectileSpread() {

		// Fires 5 projectiles simultaneously in an upwards arc
		GameObject projectileA = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 45f));
		GameObject projectileB = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 60f));
		GameObject projectileC = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 75f));
		GameObject projectileD = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 90f));
		GameObject projectileE = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 105f));
		GameObject projectileF = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 120f));
		GameObject projectileG = Instantiate (normalProjectile, gameObject.transform.position, Quaternion.Euler (0f, 0f, 135f));

		// Adds blob's current speed to projectile's starting speed so the projectiles follow the blob's movement's at the start (somewhat)
		projectileA.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileB.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileC.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileD.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileE.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileF.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);
		projectileG.GetComponent<BlobProjectileController> ().setLauncherSpeed ((targetMovement.magnitude > enemySpeed ? targetMovement.normalized * enemySpeed : targetMovement) / Time.deltaTime);

	}

}
