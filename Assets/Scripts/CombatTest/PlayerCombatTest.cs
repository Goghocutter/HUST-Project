using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatTest : MonoBehaviour
{
    [SerializeField] private GameObject hitBox;
    [SerializeField] private Transform hitBoxPosition;
    [SerializeField] private float comboCooldown = 1f;

    private Vector3 originalScale;
    private Vector3 newScale;
    private Coroutine cooldownCoroutine;

    private bool waiting;

    private void Start()
    {
        originalScale = gameObject.transform.localScale;
        newScale = originalScale;
    }

    public void CreateAttackHitbox()
    {
        //Debug.Log("Create Hitbox");
        Instantiate(hitBox, hitBoxPosition.position, hitBoxPosition.rotation);

        if (waiting)
        {
            newScale.x *= -1;
            gameObject.transform.localScale = newScale;
            CheckCooldown();
            return;
        }
        else
        {
            gameObject.transform.localScale = originalScale;
            CheckCooldown();
        }
    }

    IEnumerator Wait(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        cooldownCoroutine = null;
        waiting = false;
    }

    private void CheckCooldown()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(Wait(comboCooldown));
    }
    
}
