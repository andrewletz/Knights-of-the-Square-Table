using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
	walk,
	attack,
	interact
}

public class PlayerController : MonoBehaviour {

	public float speed = 10.0f;
	public PlayerState currentState;

	private Rigidbody rigidBody;
	private Animator animator;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		rigidBody = GetComponent<Rigidbody>();
	}

	void Update() {
		if (Input.GetButtonDown("attack") && currentState != PlayerState.attack) {
			StartCoroutine(AttackCo());
		} else if (currentState == PlayerState.walk) {
			UpdateAnimationsAndMove();
		}
	}

	void UpdateAnimationsAndMove() {
		Vector3 change = Vector3.zero;
		change.x = Input.GetAxisRaw("Horizontal");
		change.y = Input.GetAxisRaw("Vertical");

        Vector3 horizontalMovement = new Vector3(speed * Time.deltaTime * change.x, 0, 0);
        Vector3 verticalMovement = new Vector3(0, 0, speed * Time.deltaTime * change.y);
        rigidBody.MovePosition(transform.position + horizontalMovement + verticalMovement);

		if (change != Vector3.zero){
			animator.SetFloat("moveX", change.x);
			animator.SetFloat("moveY", change.y);
			animator.SetBool("moving", true);
		} else {
			animator.SetBool("moving", false);
		}
	}

	private IEnumerator AttackCo() {
		animator.SetBool("attacking", true);
		currentState = PlayerState.attack;
		yield return null;
		animator.SetBool("attacking", false);
		yield return new WaitForSeconds(.3f);
		currentState = PlayerState.walk;
	}

}
