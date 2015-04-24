using UnityEngine;
using System.Collections;

public class CableVine : StationaryEnemy {
	
	//protected Stun stunDebuff;
	protected GenericDoT constrict;
	bool inStealth;
	Rigidbody joints;
	protected Transform origin;
	protected float pull_velocity = 0.1f;
	private float maxApproachRadius;
	protected Blink blink;
	protected Vector3 MyMawPos;
	HingeJoint tether;
//	CableMaw MyMum;

	protected override void Awake () {
		base.Awake();
	}

	protected override void Start() {
		base.Start ();
		tether = GetComponentInChildren <HingeJoint> ();
		//stunDebuff = new Stun ();
		constrict = new GenericDoT (1);
		maxApproachRadius = GetComponentInChildren<SphereCollider> ().radius;
		this.blink = this.inventory.items[inventory.selected].GetComponent<Blink> ();
		if (this.blink == null) Debug.LogWarning ("Cable Vine does not have Blink equipped");

		MyMawPos.x = -5.93f; MyMawPos.y = 0f; MyMawPos.z = 10.47f;
	}

	protected override void initStates() {
	
		base.initStates ();

		State outranged = new State ("outranged");
		sM.states.Add (outranged.id, outranged);

		Transition tOutranged = new Transition (outranged);
		sM.transitions.Add (tOutranged.targetState.id, tOutranged);

		tOutranged.addCondition (this.outRanged);

		outranged.addAction (this.redeploy);

		this.addTransitionToExisting ("search", tOutranged);
		this.addTransitionToExisting ("rest", tOutranged);

		this.addTransitionToNew ("search", outranged);
		this.addTransitionToNew ("attack", outranged);
		this.addTransitionToNew ("approach", outranged);

	}

	protected override void setInitValues() {
		base.setInitValues ();
	}
	
	protected override void Update () {
		base.Update ();
		GameObject herro = aggroT.GetTopAggro();
		//Debug.Log (this.stats.health);
		//Debug.Log (this.transform.position);
	}

	protected override void Approach() {
		base.Approach ();
		if (MyMawPos != null) {
			target.transform.position = target.transform.position - pullVelocity (MyMawPos);
			Debug.Log (MyMawPos);
		} else { 
			target.transform.position = target.transform.position - pullVelocity (this.transform.position);
			Debug.Log (this.transform.position);
		}

		target.GetComponent<Player> ().BDS.addBuffDebuff (constrict, this.gameObject);
	}

	protected override void Attack ()
	{
		base.Attack ();
		// target.GetComponent<Player> ().BDS.addBuffDebuff (constrict, this.gameObject);
	}

	protected override void Rest() {
		base.Rest ();
	}

	protected void redeploy () {
		this.facing = this.target.transform.position - this.transform.position;
		this.facing.y = 0.0f;
		float wait = 1.5f;

		if (this.blink.curCoolDown <= 0) {
			do {
				/*
				this.facing.x =  Random.value * (this.facing.x == 0 ? (Random.value - 0.5f) : Mathf.Sign);
				this.facing.z = Random.value * (this.facing.z == 0 ? (Random.value - 0.5f) : Mathf.Sign);*/
	
				this.facing.x += Mathf.Sign (this.facing.x) * Random.value * 10;
				this.facing.z += Mathf.Sign (this.facing.z) * Random.value * 10;
				
				this.facing.Normalize ();
			} while (this.facing == Vector3.zero);

			this.blink.useItem ();
		}

	}

	protected bool outRanged() {
		if (this.target != null) {
			float distance = this.distanceToPlayer(this.target);
			return distance > this.maxApproachRadius && this.isHit;
		}
		return false;
	}

	private Vector3 pullVelocity(Vector3 destination){
		Vector3 dir = target.transform.position - destination;
		float time = dir.magnitude/pull_velocity;
		Vector3 velocity = new Vector3 ();
		velocity.x = dir.x / time;
		velocity.y = dir.y / time;
		velocity.z = dir.z / time;
		return velocity;
	}

	public override void damage(int dmgTaken, Character striker) {
		base.damage (dmgTaken, striker);
		target = striker.gameObject;
	}

	protected bool hasMaw() {
		return true;
	}

}
