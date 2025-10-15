using System.Collections;
using UnityEngine;

public class DestroyParticleAfterDone : MonoBehaviour
{
    void Update()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
    }
}
