using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : Manager
{
    #region Variables

    static ObjectPoolManager instance = null;

    [SerializeField] private Transform rootObjectPool;

    [Header("Pool Prefabs"), Space]
    [SerializeField] PlayerShip playerShipPrefab;
    [SerializeField] Asteroid asteroidPrefab;
    [SerializeField] FlyingSaucer flyingSaucerPrefab;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] InputHandler inputHandlerPrefab;
    [SerializeField] ParticleSystem explosionParticlePrefab;

    Dictionary<EObjectPooling, ObjectPool<GameObject>> pools;

    #endregion

    #region Properties

    static public ObjectPoolManager Instance { get { return instance; } }

    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (instance == null) { instance = this; }

        pools = new Dictionary<EObjectPooling, ObjectPool<GameObject>>();

        pools[EObjectPooling.PlayerShip] = new ObjectPool<GameObject>(CreateShip, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
        pools[EObjectPooling.Asteroid] = new ObjectPool<GameObject>(CreateAsteroid, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
        pools[EObjectPooling.FlyingSaucer] = new ObjectPool<GameObject>(CreateFlyingSaucer, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
        pools[EObjectPooling.Bullet] = new ObjectPool<GameObject>(CreateBullet, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 50);
        pools[EObjectPooling.InputHandler] = new ObjectPool<GameObject>(CreateInputHandler, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, 1, 1);
        pools[EObjectPooling.ExplosionParticle] = new ObjectPool<GameObject>(CreateExplosionParticle, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
    }

    #endregion

    #region Pool Functions

    public GameObject GetPooledObject(EObjectPooling objectPoolType)
    {
        GameObject returnObject = null;
        if (pools.ContainsKey(objectPoolType)) { returnObject = pools[objectPoolType].Get(); }

        return returnObject;
    }

    public void ReturnObject(EObjectPooling objectPoolType, GameObject returnObj)
    {
        if (returnObj == null || returnObj.transform.parent == rootObjectPool) { return; }

        if (pools.ContainsKey(objectPoolType))
        {
            pools[objectPoolType].Release(returnObj);
        }
        else
        {
            Destroy(returnObj);
        }
    }

    #region Create Functions

    private GameObject CreateShip()
    {
        return Instantiate(playerShipPrefab.gameObject);
    }

    private GameObject CreateAsteroid()
    {
        return Instantiate(asteroidPrefab.gameObject);
    }

    private GameObject CreateFlyingSaucer()
    {
        return Instantiate(flyingSaucerPrefab.gameObject);
    }

    private GameObject CreateBullet()
    {
        return Instantiate(bulletPrefab.gameObject);
    }

    private GameObject CreateInputHandler()
    {
        return Instantiate(inputHandlerPrefab.gameObject);
    }

    private GameObject CreateExplosionParticle()
    {
        GameObject instantiatedObject = Instantiate(explosionParticlePrefab.gameObject);
        ParticlePool particlePool = instantiatedObject.AddComponent<ParticlePool>();
        if (instantiatedObject.TryGetComponent<ParticleSystem>(out ParticleSystem partSys)) { particlePool.AttachParticleSystem(partSys); }
        return instantiatedObject;
    }

    #endregion


    void OnReturnedToPool<T>(T poolObject)
    {
        if (poolObject is GameObject)
        {
            GameObject gameObject = poolObject as GameObject;
            gameObject.transform.SetParent(rootObjectPool, false);
            gameObject.SetActive(false);
        }
    }


    void OnTakeFromPool<T>(T poolObject)
    {
        if (poolObject is GameObject)
        {
            GameObject gameObject = poolObject as GameObject;
            gameObject.transform.SetParent(null);
            gameObject.SetActive(true);
        }
    }

    void OnDestroyPoolObject<T>(T poolObject)
    {
        if (poolObject is GameObject)
        {
            Destroy((poolObject as GameObject));
        }
    }

    #endregion
}