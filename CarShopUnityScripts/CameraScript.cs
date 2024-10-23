using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform Car;
    [SerializeField] private Vector3 _offset = new Vector3();
    [SerializeField] private float _speed;
    [SerializeField] private float _speedRotate;

    private void FixedUpdate()
    {
        Vector3 targetPosition = Car.TransformPoint(_offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, _speed * Time.fixedDeltaTime);

        Vector3 direction = Car.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _speedRotate * Time.fixedDeltaTime);
    }
}
