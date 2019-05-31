using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public GameObject target;
    public float stopDistance = 1.0f;
    public int maxEnemyHealth = 100;


    private GameObject gameManagerObject;
    private int enemyHealth;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    private Transform healthBar;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stopDistance;
        anim = GetComponentInChildren<Animator>();

        gameManagerObject = GameObject.Find("GameManager");

        healthBar = this.transform.Find("Bar");
        enemyHealth = maxEnemyHealth;

        // Remove this and only detect player collisions
        // this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;

           
    }

    void Update() {
        if (!target){
            return;
        }

        navMeshAgent.SetDestination(target.transform.position);
        
        if (target.active == true) {
            if (navMeshAgent.velocity == Vector3.zero) {
                anim.SetBool("enemyMoving", false);
            } else {
                anim.SetBool("enemyMoving", true);
                anim.SetFloat("moveX", navMeshAgent.velocity.x);
            }        
        } else {
            anim.SetBool("enemyMoving", false);
            anim.SetBool("enemyAttacking", false);
        }


    }

    public void Death() {
        anim.SetBool("enemyMoving", false);
        anim.SetBool("enemyAttacking", false);
        anim.SetBool("isDed", true);
        this.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        navMeshAgent.isStopped = true;
        StartCoroutine(deathCo());
    }

    public void Hit(int damage) {
        enemyHealth = enemyHealth - damage;
        float healthPct = Mathf.Max((float)enemyHealth / maxEnemyHealth, 0.0f);
        healthBar.localScale = new Vector3(healthPct, 1);
        
        if (healthPct == 0.0f)
            Death();
    }

    IEnumerator deathCo() {
        yield return new WaitForSeconds(1.5f);
        gameManagerObject.GetComponent<GameManager>().EnemyDeath(this.transform.position);
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}