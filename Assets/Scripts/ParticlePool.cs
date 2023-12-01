using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    ParticleSystem particleSys;
    // Start is called before the first frame update
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
         if (particleSys != null)
        {
            if (!particleSys.IsAlive())
            {
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnObject(EObjectPooling.ExplosionParticle, gameObject);
    }
}
