using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamePillar : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip initialNoise;
    public AudioClip tickNoise;

    private int damage = 15;

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

    private void PlayNoise()
    {
        audioSource.PlayOneShot(initialNoise);
    }

    private void Tick()
    {
        audioSource.PlayOneShot(tickNoise, 0.8f);
        foreach (Collider mob in mobs)
        {
            if (mob != null)
            {
                mob.GetComponent<EnemyController>().Hit(damage);
            }
        }
    }
}
