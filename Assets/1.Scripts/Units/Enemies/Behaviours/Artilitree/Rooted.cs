// Artilitree target behaviour
using UnityEngine;

public class Rooted : ArtilleryBehaviour {

	protected RigidbodyConstraints constraints;

	// This will be called when the animator first transitions to this state.
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("Charging", false);
		this.unit.rb.isKinematic = true;
		if (this.artillery.curCircle == null) animator.SetBool ("GotCircle", false);
	}
	
	// This will be called once the animator has transitioned out of the state.
	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.unit.rb.constraints = this.constraints;
	}
	
	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (this.artillery.curCircle == null) animator.SetBool ("GotCircle", false);
	}
}