using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * _rotationSpeed);
    }
}
