using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;

    void Start()
    {
        StartCoroutine(LoopClips());
    }

    public IEnumerator LoopClips()
    {
        int rand = Random.Range(0, clips.Length);
        audioSource.clip = clips[rand];
        audioSource.Play();

        // wait for file to finish
        yield return new WaitForSeconds(audioSource.clip.length);

        // restart loop function
        StartCoroutine(LoopClips());
    }
}
