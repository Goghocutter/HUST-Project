using UnityEngine;

public class HealthBarTest : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private Transform _healthBar;

    void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        _healthBar.transform.rotation = _camera.transform.rotation;
    }
}