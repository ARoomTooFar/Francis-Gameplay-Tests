// Upgrade system for the monsters

using UnityEngine;
using System.Collections;

public class MonsterUpgradeSystem {

	// The tier of the monster
	public int tier {get; protected set;}
	
	// Base cost of monster which is multiplied by the rank of monster+1
	protected int baseCost;
	public int cost {
		get {return this.baseCost * (this.tier + 1);} // Test formula for now
	}
	
	// Return total cost of unit so far (For reselling purposes and possibly level difficulty calculations)
	public int totalValue {
		get {
			int total = 0;
			for (int t = this.tier; t >= 0; t--) total = total + (this.baseCost * (t + 1)); // Based on our test formula ^
			return total;
		}
	}
	
	//-----------//
	// Functions //
	//-----------//
	
	// Constructor
	public MonsterUpgradeSystem(int tier, int baseCost) {
		this.tier = tier;
		this.baseCost = baseCost;
	}
	
	// Ranks up the unit by one
	public void Upgrade() {
		this.tier++;
	}
}