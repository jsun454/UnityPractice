using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobContactDamage : MonoBehaviour {

	public float damage;
	public float damageRate;
	private float nextDamage;

	private PolygonCollider2D polygonCollider;

	// Use this for initialization
	void Start () {
		polygonCollider = GetComponent<PolygonCollider2D> ();
		nextDamage = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerStay2D(Collider2D other) {
		if (polygonCollider.IsTouching (other)) {
			if (other.tag == "Player" && nextDamage < Time.time) {
				PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth> ();
				playerHealth.changeHealth (-damage);
				nextDamage = Time.time + damageRate;
			}
		}
	}

}
