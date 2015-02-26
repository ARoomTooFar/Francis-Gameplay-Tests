using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TestSwordEnemy: Enemy {

	public bool alerted = false;
	public float fov = 150f;
	float lineofsight = 15f;

	public int waypointIndex = 0;

	protected float maxAtkRadius, minAtkRadius;

	// Waypoints for patrolling
	public List<Transform> patrolWP = new List<Transform>();

	protected Vector3? searchPosition = null;
	protected Vector3? lastSeenPosition = null;

	// Nav Mesh Positions
	public Vector3 resetpos;
	public Vector3 retreatPos;

	public AggroRange aRange;

	// Protected Variables
	protected NavMeshAgent nav;
	protected AggroTable aggroT;
	protected StateMachine sM;
	
	// Variables for use in player detection
	public GameObject target;
	private GameObject[] players;

	private float posTimer = 0f;
	public float aggroTimer = 10f;
	
	//-------------------//
	// Primary Functions //
	//-------------------//

	// Get players, navmesh and all colliders
	protected override void Awake ()
	{
		base.Awake ();
		facing = Vector3.back;
		aggroT = new AggroTable();
		nav = GetComponent<NavMeshAgent> ();

		retreatPos = transform.position;
		resetpos = transform.position;
		patrolWP.Add (transform);
		
		//Placeholder for more advanced aggro where target may change
		// players = GameObject.FindGameObjectsWithTag ("Player");
		// target = players [0];

		aRange.opposition = this.opposition;

		//State machine initialization
		sM = new StateMachine ();
		initStates ();
		sM.Start ();
	}

	protected override void Update()
	{
		base.Update ();
		if (target != null) target = aggroT.getTarget ();
		if(target == null || !lastSeenPosition.HasValue){
			sM.Update();
			return;
		}
		bool iseeyou = canSeePlayer (target);
		if (target && lastSeenPosition.HasValue && !iseeyou) {
			posTimer += Time.deltaTime;
		} else if (target && iseeyou){
			posTimer = 0f;
		}
		if(posTimer > aggroTimer)
		{
			posTimer = 0f;
			lastSeenPosition = null;
			alerted = false;
			target = null;
		}
		
		//Speed updates from stats now, fix navigation to not overshoot like it does
		nav.speed = stats.speed;
		
		sM.Update ();
	}
	
	// Initializes states, transitions and actions
	protected virtual void initStates(){
		
		// Initialize all states
		State rest = new State("rest");
		State approach = new State("approach");
		State attack = new State ("attack");
		State atkAnimation = new State ("attackAnimation");
		State search = new State ("search");


		// Add all the states to the state machine
		sM.states.Add (rest.id, rest);
		sM.states.Add (approach.id, approach);
		sM.states.Add (attack.id, attack);
		sM.states.Add (atkAnimation.id, atkAnimation);
		sM.states.Add (search.id, search);


		// Set initial state for the State Machine of this unit
		sM.initState = rest;


		// Initialize all transitions
		Transition tRest = new Transition(rest);
		Transition tApproach = new Transition (approach);
		Transition tAttack = new Transition (attack);
		Transition tAtkAnimation = new Transition(atkAnimation);
		Transition tSearch = new Transition(search);


		// Set conditions for the transitions
		tApproach.addCondition(isApproaching, this);
		tRest.addCondition (isResting, this);
		tAttack.addCondition (isAttacking, this);
		tAtkAnimation.addCondition (isInAtkAnimation, this);
		tSearch.addCondition (isSearching, this);


		// Set actions for the states
		rest.addAction (Rest, this);
		approach.addAction (Approach, this);
		attack.addAction (Attack, this);
		atkAnimation.addAction (AtkAnimation, this);
		search.addAction (Search, this);


		// Set the transitions for the states
		rest.addTransition (tApproach);
		approach.addTransition (tAttack);
		approach.addTransition (tSearch);
		attack.addTransition (tAtkAnimation);
		atkAnimation.addTransition (tApproach);
		atkAnimation.addTransition (tAttack);
		atkAnimation.addTransition (tSearch);
		search.addTransition (tApproach);
		search.addTransition (tAttack);
		
		/*

		State retreat = new State ("retreat");



		Transition tRetreat = new Transition (retreat);
		Transition tSearch = new Transition (search);



		tRetreat.addCondition (isRetreating, this);
		tSearch.addCondition (isSearching, this);



		approach.addTransition (tSearch);

		attack.addTransition (tRetreat);

		attack.addTransition (tSearch);
		retreat.addTransition (tRest);
		retreat.addTransition (tAttack);
		search.addTransition (tRest);
		search.addTransition (tApproach);
		search.addTransition (tAttack);


		search.addAction (Search, this);

		retreat.addAction (Retreat, this);
		*/
	}

	protected override void setInitValues() {
		base.setInitValues();
		//Testing with base 0-10 on stats with 10 being 100/cap%
		stats.maxHealth = 40;
		stats.health = stats.maxHealth;
		stats.armor = 0;
		stats.strength = 10;
		stats.coordination=0;
		stats.speed=9;
		stats.luck=0;

		this.minAtkRadius = 0.0f;
		this.maxAtkRadius = 5.0f;

		testDmg = 0;
	}
	
	//----------------------//

	//----------------------//
	// Transition Functions //
	//----------------------//

	protected virtual bool isResting(Character a) {
		TestSwordEnemy agent = (TestSwordEnemy)a;
		return (this.lastSeenPosition == null);
	}

	protected virtual bool isApproaching(Character a) {
		nav.speed = stats.speed * stats.spdManip.speedPercent;

		// If we don't have a target currently and aren't alerted, automatically assign anyone in range that we can see as our target
		if (this.target == null) {// && !this.alerted) {
			if (aRange.inRange.Count > 0) {
				foreach(Character tars in aRange.inRange) {
					if (this.canSeePlayer(tars.gameObject)) {
						this.alerted = true;
						target = tars.gameObject;
						break;
					}
				}

				if (target == null) {
					return false;
				}
			} else {
				return false;
			}
		}

		float distance = this.distanceToPlayer(this.target);

		if (distance >= this.maxAtkRadius && this.canSeePlayer (this.target) && !isInAtkAnimation(a)) {
			// agent.alerted = true;
			return true;
		}
		return false;
	}

	protected virtual bool isAttacking(Character a) {
		if (this.target != null) {
			float distance = this.distanceToPlayer(this.target);
			return distance < this.maxAtkRadius && distance >= this.minAtkRadius;
		}
		return false;
	}

	protected virtual bool isInAtkAnimation(Character a) {
		return this.attacking || this.animSteHash == this.atkHashChgSwing || this.animSteHash == this.atkHashCharge;
	}

	protected virtual bool isSearching(Character a) {
		return (this.lastSeenPosition.HasValue) && !(this.canSeePlayer (this.target) && this.alerted);
	}

	//---------------------//


	//------------------//
	// Action Functions //
	//------------------//

	protected virtual void Rest(Character a) {
		TestSwordEnemy agent = (TestSwordEnemy)a;
		// Patrol (a);
	}

	protected virtual void Patrol(Character a) {
		nav.speed = (stats.speed * stats.spdManip.speedPercent)/2;
		this.animator.SetBool ("Moving", true);
		
		if (this.nav.remainingDistance < this.nav.stoppingDistance) {
			if(this.waypointIndex <= this.patrolWP.Count - 1){
				this.waypointIndex++;
			}
			else this.waypointIndex = 0;
			
		}
		
		// if(agent.patrolWP.Count > 0) agent.nav.destination = agent.patrolWP[waypointIndex].position;
	}

	protected virtual void Approach(Character a) {
		this.facing = this.target.transform.position - this.transform.position;
		this.facing.y = 0.0f;
		this.rigidbody.velocity = this.facing.normalized * stats.speed * stats.spdManip.speedPercent;
	}

	protected virtual void Attack(Character a) {
		if (this.actable && !attacking){
			this.gear.weapon.initAttack();
		}
	}

	// We can have some logic here, but it's mostly so our unit is still during and attack animation
	protected virtual void AtkAnimation(Character a) {
	}
	
	protected virtual void Search(Character a) {
		target = null;
		if (this.lastSeenPosition.HasValue) {
			this.facing = this.lastSeenPosition.Value - this.transform.position;
			this.facing.y = 0.0f;
			this.lastSeenPosition = null;
		} else {
			this.facing  = new Vector3(Random.value, 0.0f, Random.value);
			this.facing.Normalize();
			this.rigidbody.velocity = (this.facing.normalized * stats.speed * stats.spdManip.speedPercent)/2;
		}
	}

	//------------------//


	//-----------------------//
	// Calculation Functions //
	//-----------------------//

	protected virtual bool canSeePlayer(GameObject p) {
		// Check angle of forward direction vector against the vector of enemy position relative to player position
		Vector3 direction = p.transform.position - transform.position;
		float angle = Vector3.Angle(direction, this.facing);
		
		if (angle < fov) {
			RaycastHit hit;
			if (Physics.Raycast (transform.position + transform.up, direction.normalized, out hit, lineofsight)) {
				
				if (hit.collider.gameObject == p) {
					aggroT.add(p,1);
					lastSeenPosition = p.transform.position;
					alerted = true;
					return true;
				}
			}
		}

		return false;
	}

	protected virtual float distanceToPlayer(GameObject p) {
		if (p == null) return 0.0f;
		Vector3 distance = p.transform.position - transform.position;
		return distance.sqrMagnitude;
	}

	//-----------------//

	//Public variables to tweak in inspector

	/*
	public float patrolSpeed = 2f;
	public float approachSpeed = 5f;
	public float reactionTime = 5f;			// Time buffer between player sighting and giving chase
	public float patrolWaitTime = 1f;		// Time wait when reaching the patrol way point
	
	public AggroRange awareness;

	
	
	public bool playerInSight = false;
	
	private StateMachine testStateMachine;*/

	/*
	
	public bool Alerted(){
		return alerted;
	}

	
	public bool isRetreating(Character a)
	{
		TestSwordEnemy agent = (TestSwordEnemy)a;
		float distance = agent.distanceToPlayer(agent.giveTarget());
		return distance <= 5.5f;
	}

	
	
	//Improve retreat AI
	public void Retreat(Character a)
	{
		TestSwordEnemy agent = (TestSwordEnemy)a;
		agent.alerted = true;
		agent.animator.SetBool ("Moving", true);
		agent.nav.speed = 5;
		agent.nav.destination = agent.retreatPos;
	}


	public override void damage(int dmgTaken, Character striker) {
		base.damage(dmgTaken, striker);
		aggroT.add (striker.gameObject, dmgTaken);
	}

	

	
	public Vector3? giveLastSeenPos()
	{
		return lastSeenPosition;
	}
	
	public GameObject giveTarget()
	{
		return target;
	}
	*/
}
