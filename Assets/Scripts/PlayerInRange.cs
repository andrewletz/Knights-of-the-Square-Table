﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRange : MonoBehaviour
{

	private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("Player")) {
    		animator.SetBool("enemyAttacking", true);
    	}
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            animator.SetBool("enemyAttacking", false);
        }
    }
}
