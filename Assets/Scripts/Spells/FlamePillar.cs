using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamePillar : MonoBehaviour
{
	private int damage = 10;

    private List<Collider> mobs = new List<Collider>();
    
    private void OnTriggerEnter(Collider other) {
    	if (other.CompareTag("mortal")) {
            mobs.Add(other);
    	}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && mobs.Contains(other))
        {
            mobs.Remove(other);
        }
    }

    private void Tick()
    {
        foreach (Collider mob in mobs)
        {
            if (mob != null)
            {
                mob.GetComponent<EnemyController>().Hit(damage);
            }
        }
    }
}
