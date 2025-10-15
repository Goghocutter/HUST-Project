using UnityEngine;

public class HitBoxTest : MonoBehaviour
{
    [SerializeField] private float baseDamage = 1.0f;
    private float normalDMG;
    public float _normalDMG => normalDMG;
    private float critDMG;
    public float _critDMG => critDMG;

    private float destroyDelay = 0.1f;

    private void Start()
    {
        normalDMG = baseDamage;
        critDMG = baseDamage * 2;
    }

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
            Destroy(gameObject);
        }
        else
        {
            if (other.CompareTag("Hurtbox"))
            {
                Destroy(gameObject);
            }
        }
    }

}
