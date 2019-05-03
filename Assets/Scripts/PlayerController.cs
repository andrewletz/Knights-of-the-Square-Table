using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 10.0f;

    private Rigidbody rigidBody;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        move();
    }

    void move() {
        Vector3 horizontalMovement = new Vector3(speed * Time.deltaTime * Input.GetAxisRaw("Horizontal"), 0, 0);
        Vector3 verticalMovement = new Vector3(0, 0, speed * Time.deltaTime * Input.GetAxisRaw("Vertical"));
        rigidBody.MovePosition(transform.position + horizontalMovement + verticalMovement);
    }
}
