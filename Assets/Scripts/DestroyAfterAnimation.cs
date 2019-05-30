using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    private void Destroy()
    {
        Destroy(GetComponent<Animator>());
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(this.gameObject);
    }
}
