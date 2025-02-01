using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private Transform _centreOfMass;
    [SerializeField] private Wheel[] _wheels;

    [SerializeField] private int _motorForce;
    [SerializeField] private int _brakeForce;
    [SerializeField] private float _brakeInput;
    [SerializeField] private float _slipAllowance;

    private float _verticalInput;
    private float _horizontalInput;

    public float Speed { get; private set; }
    [SerializeField] private AnimationCurve _steeringCurve;

    [SerializeField] private ParticleSystem _wheelSmokePrefab;

    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _backLights;

    private Material _backLightsMat;
    private bool _isBraking = false;

    private void Start()
    {
        for (int i = 0; i < _wheels.Length; i++)
        {
            _wheels[i].WheelSmoke = 
                Instantiate(_wheelSmokePrefab, _wheels[i].WheelCollider.transform.position - Vector3.up * _wheels[i].WheelCollider.radius, 
                Quaternion.identity, _wheels[i].WheelCollider.transform)
                .GetComponent<ParticleSystem>();
        }

        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = _centreOfMass.position;

        var materials = _body.GetComponent<Renderer>().materials;

        _backLightsMat = materials
            .FirstOrDefault(m => m.name == "BackLightsMat (Instance)");

        if (_backLightsMat is null)
        {
            string warningMessage = "Материал для задних фар не найден! Список материалов: ";

            foreach (var material in materials)
                warningMessage += material.name + ", ";

            Debug.LogWarning(warningMessage);
        }
        else
            BackLightsOn(false);
    }

    private void Update()
    {
        Move();
        Brake();

        Steering();
        CheckInput();

        CheckParticles();
    }

    private void Move()
    {
        Speed = _rb.velocity.magnitude;

        foreach (Wheel wheel in _wheels)
        {
            wheel.WheelCollider.motorTorque = _motorForce * _verticalInput;
            wheel.UpdateMeshPosition();
        }
    }

    private void CheckInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        float movingDirectional = Vector3.Dot(transform.forward, _rb.velocity);
        _brakeInput = (movingDirectional < -0.5f && _verticalInput > 0) || (movingDirectional > 0.5f && _verticalInput < 0) ? Mathf.Abs(_verticalInput) : 0;
    }

    private void BackLightsOn(bool active)
    {
        _backLightsMat.SetColor("_EmissionColor", active ? new Color(1, 30f / 255f, 0) : new Color(0, 0, 0));
        _backLights.SetActive(active);
    }

    private void Brake()
    {
        foreach (Wheel wheel in _wheels)
            wheel.WheelCollider.brakeTorque = _brakeInput * _brakeForce * (wheel.IsForwardWheels ? 0.7f : 0.3f);

        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        bool isCurrentlyBraking = localVelocity.z >= 0 ? Input.GetKey(KeyCode.S) : Input.GetKey(KeyCode.W);

        if (isCurrentlyBraking != _isBraking)
        {
            BackLightsOn(isCurrentlyBraking);
            _isBraking = isCurrentlyBraking;
        }
    }

    private void Steering()
    {
        float steeringAngle = _horizontalInput * _steeringCurve.Evaluate(Speed);
        float slipAngle = Vector3.Angle(transform.forward, _rb.velocity - transform.forward);

        if (slipAngle < 120)
            steeringAngle += Vector3.SignedAngle(transform.forward, _rb.velocity, Vector3.up);

        steeringAngle = Mathf.Clamp(steeringAngle, -48, 48);

        foreach (Wheel wheel in _wheels)
        {
            if (wheel.IsForwardWheels)
                wheel.WheelCollider.steerAngle = steeringAngle;
        }
    }

    private void CheckParticles()
    {
        foreach (Wheel wheel in _wheels)
        {
            WheelHit wheelHit;
            wheel.WheelCollider.GetGroundHit(out wheelHit);

            if (Mathf.Abs(wheelHit.sidewaysSlip) + Mathf.Abs(wheelHit.forwardSlip) > _slipAllowance)
            {
                if (wheel.WheelSmoke.isPlaying == false)
                    wheel.WheelSmoke.Play();
            }
            else
                wheel.WheelSmoke.Stop();
        }
    }
}

[System.Serializable]
public struct Wheel
{
    public Transform WheelMesh;
    public WheelCollider WheelCollider;
    public ParticleSystem WheelSmoke;
    public bool IsForwardWheels;

    public void UpdateMeshPosition()
    {
        Vector3 position;
        Quaternion rotation;

        WheelCollider.GetWorldPose(out position, out rotation);

        WheelMesh.position = position;
        WheelMesh.rotation = rotation;
    }
}