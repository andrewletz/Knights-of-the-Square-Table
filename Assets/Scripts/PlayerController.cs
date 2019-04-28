using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed = 10.0f;

	private Vector3 forward;
	private Vector3 right;

	void Start() {
		forward = Camera.main.transform.forward;
		forward.y = 0;
		forward = Vector3.Normalize(forward);
		right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
	}

	void Update() {
		move();
	}

	void move() {
		Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		Vector3 rightMovement = right * speed * Time.deltaTime * Input.GetAxis("Horizontal");
		Vector3 upMovement = forward * speed * Time.deltaTime * Input.GetAxis("Vertical");

		Vector3 mouse = Input.mousePosition;
		Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
		Vector2 relativePos = new Vector2(mouse.x - playerPos.x, mouse.y - playerPos.y);
		float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
		if (angle < 0) angle += 360;
		

		//transform.rotation = Quaternion.Euler(transform.rotation.x, -angle, transform.rotation.z);

		transform.position += rightMovement;
		transform.position += upMovement;
	}

}
