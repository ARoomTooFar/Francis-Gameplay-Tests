// AI for using sprint, this one uses it upon entering state and disables it when leaving state

using UnityEngine;

public class UseSprint : SprintBehaviour {

	// This will be called when the animator first transitions to this state.
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.unit.inventory.keepItemActive = true;
		this.sprint.useItem();
	}
	
	// This will be called once the animator has transitioned out of the state.
	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.unit.inventory.keepItemActive = false;
	}
	
	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	}
}