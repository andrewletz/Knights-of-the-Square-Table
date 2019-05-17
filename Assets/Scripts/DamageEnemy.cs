using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
	public int damage = 35;

    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("mortal")) {
    		other.GetComponent<EnemyController>().Hit(damage);
    	}
    }
}
