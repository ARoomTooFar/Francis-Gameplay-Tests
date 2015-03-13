﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class TargetCircle : MonoBehaviour {

	protected Rigidbody rigidbody;

	protected Character user;
	protected Controls controls;
	protected float speed;

	//-------------------//
	// Primary Functions //
	//-------------------//

	protected virtual void Start () {
		this.rigidbody = this.GetComponent<Rigidbody> ();
	}

	protected virtual void Update () {
		if (this.controls != null) {
			this.moveCommands();
		}
	}

	//--------------------//


	//------------------//
	// Public Functions //
	//------------------//

	// Sets the user of the target
	//     If a player, sets the controls equal to player controls
	//     If AI, they must call the move Circle function
	public virtual void setValues(Character user) {
		this.user = user;

		if (user.GetComponent<Player> () != null) {
			this.controls = user.GetComponent<Player>().controls;
		}

		this.speed = 7.5f;
	}

	// AI uses this to move circle
	public virtual void moveCircle(Vector3 move) {
		move.y = 0.0f;
		this.rigidbody.velocity = move.normalized * this.speed;
	}

	//-------------------//

	//---------------------//
	// Protected Functions //
	//---------------------//

	// How players move the circle, should be based off of their control scheme
	protected virtual void moveCommands() {
		Vector3 newMoveDir = Vector3.zero;

		//"Up" key assign pressed
		if (Input.GetKey(controls.up)) {
			newMoveDir += Vector3.forward;
		}

		//"Down" key assign pressed
		if (Input.GetKey(controls.down)) {
			newMoveDir += Vector3.back;
		}

		//"Left" key assign pressed
		if (Input.GetKey(controls.left)) {
			newMoveDir += Vector3.left;
		}

		//"Right" key assign pressed
		if (Input.GetKey(controls.right)) {
			newMoveDir += Vector3.right;
		}

		//Joystick form
		if(controls.joyUsed == 1){
			newMoveDir = new Vector3(Input.GetAxis(controls.hori),0,Input.GetAxis(controls.vert));
		}
			
		this.rigidbody.velocity = newMoveDir.normalized * this.speed;
	}

}
