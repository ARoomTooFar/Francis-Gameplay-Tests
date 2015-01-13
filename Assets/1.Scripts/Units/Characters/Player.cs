// Character class
// Base class that our heroes will inherit from

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : Character {
	
	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	protected override void setInitValues() {
		base.setInitValues();
		stats.health = 10;
		stats.armor = 0;
		stats.strength = 2;
		stats.coordination=2;
		stats.speed=10;
		stats.luck=0;
		//print("why");
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	}
}