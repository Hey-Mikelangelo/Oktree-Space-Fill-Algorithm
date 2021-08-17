using Quests.ObjectPool;
using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _asteroidPrefab;
    [SerializeField] private int _initialInstances = 50;

    private PrefabPool _pool;

    private void Awake()
    {
        _pool = new PrefabPool(_asteroidPrefab, _initialInstances);
    }

    public Asteroid SpawnAsteroid()
    {
        Asteroid asteroid = _pool.GetInstance() as Asteroid;
        asteroid.transform.parent = transform;
        return asteroid;
    }
}
