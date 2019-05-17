using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
	public int damage = 10;

    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("Player")) {
    		other.GetComponent<PlayerControllerFloating>().Hit(damage);
    	}
    }
}
