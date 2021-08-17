using Quests.ObjectPool;
using UnityEngine;

public class Asteroid : MonoBehaviour, IPoolable
{
    public MeshRenderer MeshRenderer => _meshRenderer;
    public MeshFilter MeshFilter => _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MeshFilter _meshFilter;

    private PrefabPool _pool;

    public void HandleSpawn()
    {
        gameObject.SetActive(true);
    }

    public void PrepareForPooling()
    {
        if (gameObject.TryGetComponent(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = false;
        }
        gameObject.SetActive(false);
    }

    public void SetParentPool(PrefabPool pool)
    {
        _pool = pool;
    }

    public void ReturnToPool()
    {
        _pool.AcceptReturning(this);
    }

}
