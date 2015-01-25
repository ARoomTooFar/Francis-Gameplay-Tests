﻿using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	public int damage;
	public float speed;
	public Character player;
	public ParticleSystem particles;
	public Transform target;
	// Use this for initialization
	protected virtual void Start() {
		setInitValues();
	}
	protected virtual void setInitValues() {
		particles.Play();
	}

	// Update is called once per frame
	protected virtual void Update() {
		transform.position = Vector3.MoveTowards (transform.position, target.position, speed);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Wall") {
			particles.Stop();
			Destroy(this.transform.parent.gameObject);
		}

		IDamageable<int, Vector3> component = (IDamageable<int, Vector3>) other.GetComponent( typeof(IDamageable<int, Vector3>) );
		Enemy enemy = other.GetComponent<Enemy>();
		if( component != null && enemy != null) {
			enemy.damage(damage, transform.position);
			particles.Stop();
			Destroy(this.transform.parent.gameObject);
		}
	}
}
