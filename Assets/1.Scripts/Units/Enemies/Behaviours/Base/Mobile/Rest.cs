// Rest state of enemies, atm it imitates their random search state
//     Once pathfinding is in, this can be when they reach the end of a patrol path or just the patrol path

using UnityEngine;

public class Rest : EnemyBehaviour {

	public float chgSrcDirTimer;

	// This will be called when the animator first transitions to this state.
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	}
	
	// This will be called once the animator has transitioned out of the state.
	public override void OnStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	public override void OnStateUpdate (Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// this.unit.StopSearch ();
		if (chgSrcDirTimer > 0.0f) {
			chgSrcDirTimer -= Time.deltaTime;
		} else {
			chgSrcDirTimer = 0.5f;

			this.unit.resetpos = this.unit.transform.position;
			do {
				this.unit.facing = new Vector3(Random.Range (-1.0f, 1.0f), 0.0f, Random.Range (-1.0f, 1.0f)).normalized;
			} while (this.unit.facing == Vector3.zero);

			this.unit.rb.velocity = this.unit.facing * this.unit.stats.speed * this.unit.stats.spdManip.speedPercent;
		}
	}
}