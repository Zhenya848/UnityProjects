using System.Collections;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private int _normalSpeed = 4;
    [SerializeField] private int _maxSpeed;
    private int _currentMaxSpeed;

    [SerializeField] private int _motorForce;

    [SerializeField] private int _minBrakeForce = 1;
    [SerializeField] private int _brakeForce;
    private int _currentBrakeForce;

    [SerializeField] private int _turningSpeed;
    private int _turnValue;
    private int _axleOfFrontWheelsY = 25;

    [SerializeField] private Transform[] _frontWheels;
    [SerializeField] private Transform[] _rearWheels;

    public float Speed;
    [SerializeField] private TextMeshProUGUI _speedText;

    [SerializeField] private float _maxPosX = 8f;

    private float _horizontalInput;

    private float angleOfWheelsX = 0;

    private float _currentVelocity;
    private const float _SENSIVITY_OF_ROTATION = 6.4f;

    private bool _isMobilePlatform;

    [SerializeField] private GameObject _leftAndRightButtons;

    private void Start()
    {
        _currentMaxSpeed = _normalSpeed;
        _currentBrakeForce = _minBrakeForce;

        _isMobilePlatform = Application.isMobilePlatform;

        if (_isMobilePlatform)
            StartCoroutine(TurnAngleDelay());

        _leftAndRightButtons.SetActive(_isMobilePlatform);
    }

    private void FixedUpdate()
    {
        Move();
        Brake();

        WheelsAnim();

        if (_isMobilePlatform == false)
            _horizontalInput = Input.GetAxis("Horizontal");

        CarRotate();

        if (transform.position.x >= _maxPosX)
            transform.position = new Vector3(_maxPosX, transform.position.y, transform.position.z);
        else if (transform.position.x <= -_maxPosX)
            transform.position = new Vector3(-_maxPosX, transform.position.y, transform.position.z);

        _speedText.text = $"Скорость: {Mathf.Round(Speed * 3.6f)} км/ч";
    }

    IEnumerator TurnAngleDelay()
    {
        while (true)
        {
            _horizontalInput += (_turnValue - _horizontalInput) * Time.fixedDeltaTime;

            yield return null;
        }
    }
    
    public void TurnPedal(int turnValue)
    {
        _turnValue = turnValue;
    }

    public void GasPedal(bool accelerate)
    {
        _currentMaxSpeed = accelerate ? _maxSpeed : _normalSpeed;
    }

    public void BrakePedal(bool brake)
    {
        _currentBrakeForce = brake ? _brakeForce : _minBrakeForce;
    }

    private void Move()
    {
        if (Speed >= _currentMaxSpeed)
            return;

        Speed += Time.fixedDeltaTime * _motorForce;
    }

    private void Brake()
    {
        if (Speed <= _normalSpeed + 0.1f)
            return;

        Speed -= Time.fixedDeltaTime * _currentBrakeForce;
    }

    private void WheelsAnim()
    {
        angleOfWheelsX += (Time.fixedDeltaTime * 100 * Speed) % 360;

        foreach (Transform frontWheel in _frontWheels)
            frontWheel.localRotation = Quaternion.Euler(angleOfWheelsX, _horizontalInput * _axleOfFrontWheelsY, 0);

        foreach (Transform rearWheel in _rearWheels)
            rearWheel.localRotation = Quaternion.Euler(angleOfWheelsX, 0, 0);
    }

    private void CarRotate()
    {
        transform.position += new Vector3(_horizontalInput * _turningSpeed * Time.fixedDeltaTime * (Speed / 40), 0, 0);
        transform.rotation = Quaternion
            .Euler(0, Mathf
            .Clamp(Mathf
            .SmoothDamp(0, Speed * _horizontalInput * _SENSIVITY_OF_ROTATION, ref _currentVelocity, 0.4f), -8, 8), 0);
    }
}
