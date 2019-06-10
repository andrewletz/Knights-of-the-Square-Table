using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Implosion : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip initialNoise;

    private int damage = 70;
    private int growth = 5;
    private float travelTime = 2.0f; // travel time in seconds towards center of spell
    private float damageDistance = 1.0f; // how close the mob needs to be to get damaged

    private List<Coroutine> coroutines = new List<Coroutine>();

    private void LevelUp(float multiplier)
    {
        damage += (int)(multiplier * growth);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("mortal"))
        {
            coroutines.Add(StartCoroutine(PullTowardsPosition(other, transform.parent.transform.position)));
        }
    }

    private void StopPulling()
    {
        foreach (Coroutine pull in coroutines)
        {
            StopCoroutine(pull);
        }
    }

    private void PlayNoise()
    {
        audioSource.PlayOneShot(initialNoise);
    }

    IEnumerator PullTowardsPosition(Collider mob, Vector3 targetPos)
    {
        Vector3 startPosition = mob.transform.position;

        // travel towards the click position
        float distance;
        bool damaged = false;
        float startTime = Time.time;
        float endTime = startTime + travelTime;
        yield return null;
        while (Time.time < endTime)
        {
            if (mob == null)
            {
                yield break;
            }
            float progress = (Time.time - startTime) / travelTime;
            mob.transform.position = Vector3.Lerp(startPosition, targetPos, progress);
            distance = Vector3.Distance(mob.transform.position, targetPos);
            if (distance <= damageDistance && damaged != true)
            {
                mob.GetComponent<EnemyController>().Hit(damage);
                damaged = true;
            }
            yield return null;
        }
    }
}
