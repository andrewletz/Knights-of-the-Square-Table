using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    private GameObject hitBox;

    private void Start()
    {
        hitBox = this.transform.Find("spell hit box").gameObject;
    }

    // used for Implosion
    private void StopImplosion()
    {
        hitBox.SendMessage("StopPulling");
    }

    // used for Flame Pillar
    private void Tick()
    {
        hitBox.SendMessage("Tick");
    }

    private void PlayNoise()
    {
        hitBox = this.transform.Find("spell hit box").gameObject;
        hitBox.SendMessage("PlayNoise");
    }

    private void Destroy()
    {
        Destroy(GetComponent<Animator>());
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(this.gameObject);
    }
}
