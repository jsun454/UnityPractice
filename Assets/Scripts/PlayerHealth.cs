using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public GameObject deathFX;

	public float maxHealth;
	private float currentHealth;

	public Slider healthSlider;
	public Image damageScreen;

	private bool damaged;
	private Color damagedColor = new Color(1f, 0f, 0f, 1f);
	private float smoothColor = 5f;

	// Use this for initialization
	void Start () {
		
		currentHealth = maxHealth;
		healthSlider.maxValue = maxHealth;
		healthSlider.value = maxHealth;

		damaged = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (damaged) {
			damageScreen.color = damagedColor;
		} else {
			damageScreen.color = Color.Lerp (damageScreen.color, Color.clear, smoothColor * Time.deltaTime);
		}
		damaged = false;

	}

	public void changeHealth (float health) {
		
		currentHealth = currentHealth + health < maxHealth ? currentHealth + health : maxHealth;
		healthSlider.value = currentHealth;

		if (health < 0) {
			damaged = true;
		}

		if (currentHealth <= 0) {
			playerDeath ();
		}

	}

	public void playerDeath() {
		
		Instantiate (deathFX, transform.position, transform.rotation);
		Destroy (gameObject);

	}

}
