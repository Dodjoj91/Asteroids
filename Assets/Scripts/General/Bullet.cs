using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables

    [SerializeField] private BoxCollider2D boxCollider2D;

    private Vector3 direction = Vector3.zero;
    private float speed = 2.5f;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        ManagerSystem.Instance.GameManager.ShouldRemoveBullet(this, false);
    }

    private void Update()
    {
        UpdateBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RemoveBullet();
    }

    #endregion

    #region Update Functions

    private void UpdateBullet()
    {
        transform.position = transform.position + direction * speed * Time.deltaTime;
        IsOutsideView();
    }

    #endregion

    #region Setup Functions

    private void ResetVariables()
    {
        boxCollider2D.includeLayers = 0;
        boxCollider2D.excludeLayers = 0;
        direction = Vector3.zero;
        speed = 2.5f;
    }

    private void RemoveBullet()
    {
        ResetVariables();
        ObjectPoolManager.Instance.ReturnObject(EObjectPooling.Bullet, gameObject);
        ManagerSystem.Instance.GameManager.ShouldRemoveBullet(this, true);
    }

    public void SetBulletVariables(Vector3 direction, int includeLayers, int excludeLayers, float speed)
    {
        this.direction = direction;
        this.speed = speed;

        boxCollider2D.includeLayers = 1 << includeLayers;
        boxCollider2D.excludeLayers = (1 << excludeLayers) | (1 << LayerMask.NameToLayer(StaticDefines.LAYER_BULLET));
    }

    #endregion

    #region Utility Functions

    private void IsOutsideView()
    {
        if (ExtensionUtility.TryGetInvertedOutsidePosition(transform.position, boxCollider2D.size, out Vector3 _))
        {
            RemoveBullet();
        }
    }

    #endregion
}