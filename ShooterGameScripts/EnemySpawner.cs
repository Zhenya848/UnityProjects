using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> _enemies;

    [SerializeField] private float _offsetPerSecond;
    [SerializeField] private int _maxEnemyCount;

    private int _currentEnemyCount => transform.childCount;

    public bool IsActive = true;

    private void Start()
    {
        StartCoroutine(StartSpawner());
    }

    private IEnumerator StartSpawner()
    {
        while (IsActive)
        {
            yield return new WaitForSeconds(_offsetPerSecond >= 1 ? _offsetPerSecond : 1);

            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        if (_currentEnemyCount >= _maxEnemyCount)
            return;

        var enemy = Instantiate(GetRandomEnemy().gameObject, transform.position, transform.rotation);

        enemy.transform.SetParent(transform);
    }

    private Enemy GetRandomEnemy() => _enemies[Random.Range(0, _enemies.Count)];
}
