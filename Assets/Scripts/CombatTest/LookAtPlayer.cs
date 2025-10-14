using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Camera playerCamera;
    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        Vector3 targetDirection = playerCamera.transform.position - transform.position;
        Vector3 newRotation = Vector3.RotateTowards(transform.forward, targetDirection, 10f * Time.deltaTime, 10f);

        transform.rotation = Quaternion.LookRotation(newRotation);
    }
}
