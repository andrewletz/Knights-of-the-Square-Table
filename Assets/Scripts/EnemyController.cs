using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        
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
