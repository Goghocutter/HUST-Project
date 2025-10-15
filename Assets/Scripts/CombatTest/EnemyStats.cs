using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float maxHP = 10f;
    [SerializeField] private GameObject deathEffect;
    public float _maxHP => maxHP;
    private float currentHP;
    public float _currentHP => currentHP;

    private void Start()
    {
        currentHP = maxHP;
    }


    void Update()
    {

        if(currentHP <= 0)
        {
            Dead();
        }
    }

    public void TakeDMG(float damage)
    {
        currentHP -= damage;
    }

    public void Dead()
    {
        Instantiate(deathEffect,transform.position, transform.rotation);
        Destroy(gameObject);
    }

}
