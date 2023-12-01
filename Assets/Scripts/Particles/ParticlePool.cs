using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    #region Variables

    ParticleSystem particleSys;

    #endregion

    #region Unity Functions

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

    #endregion

    #region Setup Functions

    private void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnObject(EObjectPooling.ExplosionParticle, gameObject);
    }

    public void AttachParticleSystem(ParticleSystem particleSys) => this.particleSys = particleSys;

    #endregion
}