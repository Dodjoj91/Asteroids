using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    #region Variables

    private ParticleSystem particleSys;

    #endregion

    #region Unity Functions

    private void Update()
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