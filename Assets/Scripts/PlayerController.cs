using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public enum PlayerState {
		walk,
		attack,
		interact
	}

	public float speed = 10.0f;
	public PlayerState currentState;
	public int maxPlayerHealth = 100;


	private int playerHealth;
	private Rigidbody rigidBody;
	private Animator animator;
	private Transform healthBar;
	private GameObject weapon;
    private GameObject orb;

	void Start() {
		animator = GetComponentInChildren<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		playerHealth = maxPlayerHealth;
		healthBar = this.transform.Find("Bar");

		// get our weapon object once at start time
		for (int i = 0; i < transform.childCount; i++) {
			Transform currentTransform = transform.GetChild(i).transform;
			if (currentTransform.name == "Weapon") {
				this.weapon = currentTransform.gameObject;
                this.orb = currentTransform.GetChild(1).gameObject;
			}
		}
	}

	void FixedUpdate() {
        Vector3 mousePos = Input.mousePosition;
        if (Input.GetButtonDown("attack") && currentState != PlayerState.attack) {
            //StartCoroutine(AttackCo());
            //weapon.SendMessage("StartAttackAnimation");

            // cast ray down from mouse position to find where the mouse is on the floor
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // won't be true if the normal isn't 1 (flat surface)
            if (Physics.Raycast(ray, out hit))
            {

                Vector3 target = hit.point;

                // bump the position of the mouse so the flame pillar comes out at the right spot
                target.y += 0.5f;
                target.z += 0.5f;
                orb.SendMessage("CastSpell", target);
            }
        }
        
        // send angle of mouse to weapon controller
        Vector3 screenPlayerPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 relativePos = mousePos - screenPlayerPos;
        float angle = Mathf.Atan2((float)((int)relativePos.y), (float)((int)relativePos.x)) * Mathf.Rad2Deg;
        angle += 90;
        weapon.SendMessage("GetAngle", (float) ((int) -angle)); // send the angle value to the weapon container

        if (currentState == PlayerState.walk) {
			UpdateAnimationsAndMove();
		}
	}

	void UpdateAnimationsAndMove() {
		// move
		Vector3 change = Vector3.zero;
		change.x = Input.GetAxisRaw("Horizontal");
		change.y = Input.GetAxisRaw("Vertical");

        Vector3 horizontalMovement = new Vector3(speed * Time.deltaTime * change.x, 0, 0);
        Vector3 verticalMovement = new Vector3(0, 0, speed * Time.deltaTime * change.y);
        rigidBody.MovePosition(transform.position + horizontalMovement + verticalMovement);

        // change animation
		if (change != Vector3.zero){
			animator.SetFloat("moveX", change.x);
			animator.SetFloat("moveY", change.y);
			animator.SetBool("moving", true);
		} else {
			animator.SetBool("moving", false);
		}
	}

    public void Death() {
    	GameObject gameManager = GameObject.Find("GameManager");
    	gameManager.GetComponent<GameManager>().RestartGame();
    	this.gameObject.SetActive(false);
    }

    public void Hit(int damage) {
        playerHealth = playerHealth - damage;
        float healthPct = Mathf.Max((float)playerHealth / maxPlayerHealth, 0.0f);
        healthBar.localScale = new Vector3(healthPct, 1);
        
        if (healthPct == 0.0f){
            Death();
        }
    }

	// private IEnumerator AttackCo() {
	// 	animator.SetBool("attacking", true);
	// 	currentState = PlayerState.attack;
	// 	yield return null;
	// 	animator.SetBool("attacking", false);
	// 	yield return new WaitForSeconds(.3f);
	// 	currentState = PlayerState.walk;
	// }

}