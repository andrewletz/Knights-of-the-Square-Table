using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{

    // used for Implosion
    private void StopImplosion()
    {
        this.transform.Find("spell hit box").gameObject.SendMessage("StopPulling");
    }

    // used for Flame Pillar
    private void Tick()
    {
        this.transform.Find("spell hit box").gameObject.SendMessage("Tick");
    }

    private void Destroy()
    {
        Destroy(GetComponent<Animator>());
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(this.gameObject);
    }
}
