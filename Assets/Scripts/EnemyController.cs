using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public GameObject target;
    public float stopDistance = 1.0f;

    private NavMeshAgent navMeshAgent;
    private Animator anim;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stopDistance;

        // Remove this and only detect player collisions
        this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;

        anim = GetComponentInChildren<Animator>();   
    }

    void Update() {
        navMeshAgent.SetDestination(target.transform.position);
        
        if (navMeshAgent.velocity == Vector3.zero) {
            anim.SetBool("enemyMoving", false);
        } else {
            anim.SetBool("enemyMoving", true);
        }
        anim.SetFloat("moveX", navMeshAgent.velocity.x);
    }

    public void Death() {
        anim.SetBool("isDed", true);
        this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        StartCoroutine(deathCo());
    }

    IEnumerator deathCo() {
        yield return new WaitForSeconds(1.5f);
        this.gameObject.SetActive(false);
    }
}