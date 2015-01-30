﻿// Riot Shield item

using UnityEngine;
using System.Collections;

public class RiotShield : ToggleItem {
	
	private float dmgReduction, userSlow;
	private MeshRenderer meshRenderer;
	
	// Use this for initialization
	protected override void Start () {
		base.Start();
		
		meshRenderer = GetComponent<MeshRenderer>();
	}
	
	protected override void setInitValues() {
		base.setInitValues();

		cooldown = 10.0f;
		maxDuration = 5;
		dmgReduction = 0.9f;
		userSlow = 0.75f;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();

	}
	
	// Called when character with an this item selected uses their item key
	public override void useItem() {
		base.useItem();

		// player.animator.SetTrigger("Sprint"); Set speed var in animator once we have the animation
	}

	protected override IEnumerator bgnEffect() {
		collider.enabled = true;
		meshRenderer.enabled = true;
		player.stats.dmgManip.setDamageReduction(1, dmgReduction);
		player.slow (userSlow);
		return base.bgnEffect();
	}
	
	public override void deactivateItem() {
		base.deactivateItem();
	}

	protected override void atvDeactivation() {
		collider.enabled = false;
		meshRenderer.enabled = false;

		player.stats.dmgManip.removeDamageReduction(1, dmgReduction);
		player.removeSlow (userSlow);
		base.atvDeactivation();
	}

	protected override void animDone() {
		base.animDone ();
	}
}
