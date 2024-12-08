using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    private Transform _player;
    private PlayerController _playerController;

    [SerializeField] private float _distanceOfAttack = 1;
    [SerializeField] private float _speedOfAttackPerSecond = 1;
    [SerializeField] private int _damage;

    private bool _isDamaging = false;

    Color _color = new Color();
    private Renderer _renderer;
    private float _g;

    private void Start()
    {
        var player = GameObject.FindWithTag("Player");

        _player = player.transform;
        _playerController = player.GetComponent<PlayerController>();

        _renderer = GetComponent<Renderer>();
        _color = _renderer.material.color;
        _g = _color.g;

        StartCoroutine(StartDamaging());
    }

    private void FixedUpdate()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        var distanceFromThePlayer = transform.position - _player.transform.position;

        if (distanceFromThePlayer.magnitude <= _distanceOfAttack)
        {
            _isDamaging = true;

            return;
        }
        else
            _isDamaging = false;

        transform.position -= distanceFromThePlayer.normalized * Speed / 100;
    }

    private IEnumerator StartDamaging()
    {
        while (true)
        {
            if (_isDamaging)
            {
                _playerController.GetDamage(_damage);
                yield return new WaitForSeconds(_speedOfAttackPerSecond >= 1 ? _speedOfAttackPerSecond : 1);
            }

            yield return null;
        }
    }

    private void GetDamage()
    {
        Health--;

        if (Health <= 0)
            Destroy(gameObject);

        _g -= _color.g / Health;
        _renderer.material.color = new Color(_color.r, _g, _color.b);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Bullet")
            GetDamage();
    }
}
