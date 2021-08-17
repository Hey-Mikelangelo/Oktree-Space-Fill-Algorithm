using System.Collections.Generic;
using UnityEngine;

public class AsteroidBeltGenerator : MonoBehaviour
{
    [SerializeField] private AsteroidsSpawner _asteroidsSpawner;
    [SerializeField] private BoxCollider _asteroidBeltBoundsCollider;
    [SerializeField] private float _minOctreeCellSize = 1;
    [SerializeField] private float _minAsteroidScale = 1;
    [SerializeField] private float _maxAsteroidScale = 10;
    [Tooltip("number how many times algorith tries to insert asteroid before it moves to the next asteroid")]
    [SerializeField] private int _maxInsertTriesCount = 10;
    [SerializeField] private int _asteroidsTargetCount = 200;
    [Tooltip("Asteroid bounds are calculated from mesh bounds. " +
        "Rotated asteroid can sligthtly stick out from the mesh bounds, so it is needed to " +
        "add compenstation for non spherical objects like asteroid")]

    [SerializeField] private bool _showBounds, _showOctree;

    private Octree _octree;
    private List<Asteroid> _spawnedAsteroids = new List<Asteroid>();
   private List<Bounds> _asteroidBounds = new List<Bounds>();
    private void Start()
    {
        float biggestExtents = Utils.GetBiggestVector3Component(_asteroidBeltBoundsCollider.bounds.extents);
        _octree = new Octree(_asteroidBeltBoundsCollider.bounds.min, biggestExtents * 2, _minOctreeCellSize);
        SpawnAsteroidBelt();
    }

    private void SpawnAsteroidBelt()
    {
        foreach (Asteroid asteroid in _spawnedAsteroids)
        {
            asteroid.ReturnToPool();
        }

        for (int i = 0; i < _asteroidsTargetCount; i++)
        {
            Asteroid spawndedAsteroid = TrySpawnAsteroid();
            if (spawndedAsteroid != null)
            {
                _spawnedAsteroids.Add(spawndedAsteroid);
            }
        }

        Debug.Log($"Spawned asteroids count: {_spawnedAsteroids.Count}");
    }

    private Asteroid TrySpawnAsteroid()
    {
        Asteroid asteroid = _asteroidsSpawner.SpawnAsteroid();
        for (int triesCount = 0; triesCount < _maxInsertTriesCount; triesCount++)
        {
            SetRandomTransform(asteroid);
            Bounds asteroidBounds = GetAsteroidBounds(asteroid);
            if (_octree.OverlapsOccupied(asteroidBounds) == false)
            {
                _octree.Insert(asteroidBounds);
                _asteroidBounds.Add(asteroidBounds);
                asteroid.MeshRenderer.enabled = true;
                return asteroid;
            }
        }
        asteroid.ReturnToPool();
        return null;
    }

    private void SetRandomTransform(Asteroid asteroid)
    {
        Bounds asteroidBeltBounds = _asteroidBeltBoundsCollider.bounds;

        Vector3 randomScale = Utils.GetRandomUniformScale(_minAsteroidScale, _maxAsteroidScale);
        Vector3 radomPosition = asteroidBeltBounds.GetRandomPoint();
        Quaternion randomRotation = Utils.GetRandomRotation();

        Transform asteroidTransform = asteroid.gameObject.transform;
        asteroidTransform.localScale = randomScale;
        asteroidTransform.position = radomPosition;
        asteroidTransform.rotation = randomRotation;
    }

    private void OnDrawGizmos()
    {
        if (_showBounds)
        {
            foreach (var bound in _asteroidBounds)
            {
                Gizmos.DrawWireCube(bound.center, bound.size);
            }
        }

        if (_showOctree)
        {
            _octree.Draw();
        }
       
    }

    private Bounds GetAsteroidBounds(Asteroid asteroid)
    {
        Bounds bounds = Utils.GetRotatedMeshBounds(asteroid.MeshFilter);
        return bounds;
    }

  
}
