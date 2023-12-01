using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider2D;

    Vector3 direction = Vector3.zero;
    float speed = 2.5f;

    private void OnEnable()
    {
        ManagerSystem.Instance.GameManager.SetBullet(this, false);
    }

    void Update()
    {
        UpdateBullet();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RemoveBullet();
    }

    private void UpdateBullet()
    {
        transform.position = transform.position + direction * speed * Time.deltaTime;
        IsOutsideView();
    }

    private void IsOutsideView()
    {
        if (ExtensionUtility.TryGetInvertedOutsidePosition(transform.position, boxCollider2D.size, out Vector3 _))
        {
            RemoveBullet();
        }
    }

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
        ManagerSystem.Instance.GameManager.SetBullet(this, true);
    }
    public void SetBulletVariables(Vector3 direction, int includeLayers, int excludeLayers, float speed)
    {
        this.direction = direction;
        this.speed = speed;

        boxCollider2D.includeLayers = 1 << includeLayers;
        boxCollider2D.excludeLayers = (1 << excludeLayers) | (1 << LayerMask.NameToLayer(StaticDefines.LAYER_BULLET));
    }
}
