using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _player;

    [SerializeField] private float _speed;

    [SerializeField] private int _health;
    [SerializeField] private Scrollbar _healthScrollbar;

    private float _horizontalInput;
    private float _verticalInput;

    private float _xRotCurrent;
    private float _yRotCurrent;

    private float _xVelocity;
    private float _yVelocity;

    [SerializeField] private float _delay;
    private Vector2 _tapPosition;

    private void Start()
    {
        Cursor.visible = false;
        _tapPosition = Input.mousePosition;

        _healthScrollbar.size = 1;
    }

    private void FixedUpdate()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        float horizontalSpeed = _horizontalInput * Time.fixedDeltaTime * _speed;
        float verticalSpeed = _verticalInput * Time.fixedDeltaTime * _speed;

        _player.transform.position += new Vector3(_player.transform.forward.x * verticalSpeed, 0, _player.transform.forward.z * verticalSpeed);
        _player.transform.position += new Vector3(_player.transform.right.x * horizontalSpeed, 0, _player.transform.right.z * horizontalSpeed);

        CameraRotate();
    }

    public void GetDamage(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            Time.timeScale = 0;

            //Some logic...
        }

        _healthScrollbar.size -= _healthScrollbar.size * damage / _health;
    }

    private void CameraRotate()
    {
        Vector2 newPosition = ((Vector2)Input.mousePosition - _tapPosition) / _delay;

        _xRotCurrent = Mathf.SmoothDamp(_xRotCurrent, newPosition.x, ref _xVelocity, 0.1f);
        _yRotCurrent = Mathf.SmoothDamp(_yRotCurrent, newPosition.y, ref _yVelocity, 0.1f);

        _player.transform.rotation = Quaternion.Euler(-_yRotCurrent, _xRotCurrent, 0f);
    }
}
