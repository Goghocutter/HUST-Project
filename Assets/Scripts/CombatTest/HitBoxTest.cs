using UnityEngine;

public class HitBoxTest : MonoBehaviour
{
    private float destroyDelay = 0.1f;

    private void Update()
    {
        if( destroyDelay > 0)
        {
            destroyDelay -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weakpoint"))
        {
            Debug.Log("Hit weakpoint");
            Destroy(gameObject);
        }
        else
        {
            if (other.CompareTag("Hurtbox"))
            {
                Debug.Log("Hit Hurtbox");
                Destroy(gameObject);
            }
        }
    }

}
