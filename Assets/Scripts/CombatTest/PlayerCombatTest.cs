using UnityEngine;

public class PlayerCombatTest : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    [SerializeField] private Transform hitBoxPosition;

    public void CreateAttackHitbox()
    {
        Debug.Log("Create Hitbox");
        Instantiate(hitBox, hitBoxPosition.position, hitBoxPosition.rotation);
    }
}
