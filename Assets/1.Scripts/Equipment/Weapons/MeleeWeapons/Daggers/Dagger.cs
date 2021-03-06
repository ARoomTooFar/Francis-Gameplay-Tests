// Dagger class, put into the head for now

using UnityEngine;
using System.Collections;

public class Dagger : MeleeWeapons {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Used for setting sword stats for each equipment piece
	protected override void setInitValues() {
		base.setInitValues();
		
		// Default sword stats
		stats.weapType = 1;
		stats.damage = 10;
		this.stats.buffDuration = 0.25f;
		
		stats.maxChgTime = 2;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}

	public override void collideOn () {
		base.collideOn ();
	}

	protected override void onHit(Character enemy) {
		if(stats.debuff != null && user.animator.GetFloat ("ChargeTime") < 0.5f && stats.buffDuration > 0) {
			enemy.BDS.addBuffDebuff(stats.debuff, this.user.gameObject, stats.buffDuration);
		}

		if (ARTFUtilities.IsBehind(user.transform.position, enemy.facing, enemy.transform.position)) {
			enemy.damage((int)((stats.damage + this.user.stats.strength + stats.chgDamage * (this.tier + 1)) * 1.5f), user.transform, user.gameObject);
		} else {
			enemy.damage(this.CalculateTotalDamage(), user.transform, user.gameObject);
		}

	}
}
