using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour {

	public GameObject deathFX;
	public Slider healthSlider;

	public float maxHealth;
	private float currentHealth;

	// Use this for initialization
	void Start () {
		
		currentHealth = maxHealth;
		healthSlider.maxValue = maxHealth;
		healthSlider.value = maxHealth;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void changeHealth(float health) {

		//healthSlider.gameObject.SetActive (true);
		currentHealth = currentHealth + health < maxHealth ? currentHealth + health : maxHealth;
		healthSlider.value = currentHealth;

		if (currentHealth <= 0) {
			enemyDeath ();
		}

	}

	public float getCurrentHealth() {
		return currentHealth;
	}

	void enemyDeath() {
		
		Destroy (gameObject);
		Instantiate (deathFX, transform.position, transform.rotation);

	}

}
