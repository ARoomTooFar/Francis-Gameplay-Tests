using UnityEngine;
using System.Collections;

public class Rifle : RangedWeapons {
	
	public GameObject radar;
	protected AoETargetting aoe;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	protected override void setInitValues() {
		base.setInitValues();
		
		stats.weapType = 5;
		stats.damage = 20;
		stats.maxChgTime = 3;
		stats.chargeSlow = new Slow(1.0f);

		this.spread = 0;
		this.stats.buffDuration = 0.75f;
		
		GameObject dar = (GameObject) Instantiate(radar, this.user.transform.position + radar.transform.position, Quaternion.identity);
		dar.transform.parent = this.user.transform;
		this.aoe = dar.GetComponent<AoETargetting>();
		this.aoe.affectEnemies = true;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}
	
	public override void AttackStart() {
		this.FireProjectile();
		StartCoroutine(makeSound(action,playSound,action.length));
	}
	
	public override void SpecialAttack() {
		StartCoroutine(makeSound(action,playSound,action.length));
		HomingBullet newBullet = ((GameObject)Instantiate(this.projectile, this.transform.position + this.user.facing * 2, this.user.transform.rotation)).GetComponent<HomingBullet>();
		newBullet.setInitValues(user, opposition, this.CalculateTotalDamage(), particles.startSpeed, this.stats.debuff != null, stats.debuff, this.stats.buffDuration * 2, this.user.FindClosestCharacter(this.aoe.unitsInRange));
	}
	
	protected override void FireProjectile() {
		HomingBullet newBullet = ((GameObject)Instantiate(projectile, this.transform.position + this.user.facing * 2, this.user.transform.rotation)).GetComponent<HomingBullet>();
		newBullet.setInitValues(user, opposition, this.CalculateTotalDamage(), particles.startSpeed, this.stats.debuff != null, stats.debuff, this.stats.buffDuration, this.user.FindClosestCharacter(this.aoe.unitsInRange));
	}
	
}
