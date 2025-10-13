using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HurtBoxTest : MonoBehaviour
{
    [SerializeField] private float knockback = 1f;
    [SerializeField] private float stopDuration = 0.1f;

    [SerializeField] private Material flashShader;

    [SerializeField] private string shakeAnimation;

    private GameObject parent;
    private Rigidbody parentRB;
    private Camera playerCamera;

    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Coroutine flashRoutine;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        parentRB = parent.GetComponent<Rigidbody>();
        playerCamera = FindAnyObjectByType<Camera>();
        meshRenderer = parentRB.GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox"))
        {
            Flash(); //flash
            playerCamera.GetComponent<Animator>().Play(shakeAnimation); //shake
            FindFirstObjectByType<HitStopManager>().Stop(stopDuration); //hitstop

            parentRB.AddForce(playerCamera.transform.rotation * Vector3.forward * knockback, ForceMode.Impulse);
        }

        if (other.CompareTag("Hurtbox") || other.CompareTag("Weakpoint"))
        {
            if (other.transform.parent.gameObject != transform.parent.gameObject)
            {
                Flash(); //flash
                playerCamera.GetComponent<Animator>().Play(shakeAnimation); //shake
                FindFirstObjectByType<HitStopManager>().Stop(stopDuration); //hitstop

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
