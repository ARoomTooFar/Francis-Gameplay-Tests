﻿// Projectile parent class
//     For now, it just goes straight

using UnityEngine;
using System.Collections;
using System;

public class Projectile : MonoBehaviour {
	public int damage;
	public float speed;
	public bool castEffect;
	private Character user;
	public ParticleSystem particles;
	public Transform target;
	public BuffsDebuffs debuff;

	protected Type opposition;

	// Use this for initialization
	protected virtual void Start() {

	}
	public virtual void setInitValues(Character player, Type ene, float partSpeed,bool effect,BuffsDebuffs hinder) {
		user = player;
		opposition = ene;

		transform.Rotate(Vector3.right * 90);

		damage = 1;
		speed = 0.5f;
		castEffect = effect;
		debuff = hinder;
		particles.startSpeed = partSpeed;
		particles.Play();
	}

	// Update is called once per frame
	protected virtual void Update() {
		transform.position = Vector3.MoveTowards (transform.position, target.position, speed);
	}
	
	protected virtual void onHit(Character enemy) {
		if(castEffect && debuff != null){
			/*if(stats.buffDuration > 0){
				enemy.BDS.addBuffDebuff(debuff, this.gameObject, stats.buffDuration);
			}else{*/
				enemy.BDS.addBuffDebuff(debuff, this.gameObject);
			//}
		}
		enemy.damage(damage, user);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Wall") {
			particles.Stop();
			Destroy(gameObject);
		}
		if (other.tag == "Prop") {
			other.GetComponent<Prop>().damage(damage);
			particles.Stop();
			Destroy(gameObject);
		}
		IDamageable<int, Character> component = (IDamageable<int, Character>) other.GetComponent( typeof(IDamageable<int, Character>) );
		Character enemy = (Character) other.GetComponent(opposition);
		if( component != null && enemy != null) {
			onHit(enemy);
			particles.Stop();
			Destroy(gameObject);
		} else {
			IDamageable<int, Traps> component2 = (IDamageable<int, Traps>) other.GetComponent (typeof(IDamageable<int, Traps>));
			if (component2 != null) {
				component2.damage(damage);
			}
		}
	}
}
