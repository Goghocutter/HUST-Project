using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HurtBoxTest : MonoBehaviour
{
    [SerializeField] private float knockback = 1f;
    [SerializeField] private float stopDuration = 0.1f;

    [SerializeField] private Material flashShader;

    [SerializeField] private string shakeAnimation;

    [SerializeField] private ParticleSystem onHitEffect;

    private GameObject parent;
    private Rigidbody parentRB;
    private Camera playerCamera;

    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    private EnemyStats enemyStats;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        parentRB = parent.GetComponent<Rigidbody>();
        playerCamera = FindAnyObjectByType<Camera>();
        meshRenderer = parentRB.GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;
        enemyStats = parent.GetComponent<EnemyStats>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //this part is very janky and pure hard code. Will fix later. There's also a bug that it deal crit+normal damage if you hit between both collisions
        if (other.CompareTag("Hitbox") && gameObject.tag == "Hurtbox") //normal damage hit
        {
            Flash(); //flash
            playerCamera.GetComponent<Animator>().Play(shakeAnimation); //shake
            FindFirstObjectByType<HitStopManager>().Stop(stopDuration); //hitstop

            enemyStats.TakeDMG(other.GetComponent<HitBoxTest>()._normalDMG);

            Instantiate(onHitEffect, transform.position, transform.rotation);

            parentRB.AddForce(playerCamera.transform.rotation * Vector3.forward * knockback, ForceMode.Impulse);
        }
        else if (other.CompareTag("Hitbox") && gameObject.tag == "Weakpoint") //crit damage hit
        {
            Flash(); //flash
            playerCamera.GetComponent<Animator>().Play(shakeAnimation); //shake
            FindFirstObjectByType<HitStopManager>().Stop(stopDuration); //hitstop

            enemyStats.TakeDMG(other.GetComponent<HitBoxTest>()._critDMG);

            Instantiate(onHitEffect, transform.position, transform.rotation);

            parentRB.AddForce(playerCamera.transform.rotation * Vector3.forward * knockback, ForceMode.Impulse);
        }

        if (other.CompareTag("Hurtbox") || other.CompareTag("Weakpoint"))
        {
            if (other.transform.parent.gameObject != transform.parent.gameObject)
            {
                Flash(); //flash
                playerCamera.GetComponent<Animator>().Play(shakeAnimation); //shake
                FindFirstObjectByType<HitStopManager>().Stop(stopDuration); //hitstop

                //enemyStats.TakeDMG(other.GetComponent<HitBoxTest>()._currentDMG);

                parentRB.AddForce(playerCamera.transform.rotation * Vector3.forward * knockback / 1.2f, ForceMode.Impulse);
            }
        }


    }

    private IEnumerator FlashRoutine()
    {
        meshRenderer.material = flashShader;
        yield return new WaitForSecondsRealtime(stopDuration);
        meshRenderer.material = originalMaterial;
        flashRoutine = null;
    }

    private void Flash()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine());
    }

}
