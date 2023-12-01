using UnityEngine;


public class FlyingSaucer : Unit
{
    #region Variables

    UnitDataEnemy enemyData;

    Vector2 moveDirection;
    float shootTimer = 0.0f;

    #endregion

    #region Unity Functions

    protected override void Start()
    {
        base.Start();
        enemyData = UnitDataEnemy;
        moveDirection = ExtensionUtility.GetRandomAngleDirection(8);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticDefines.TAG_BULLET))
        {
            Hit();
            DestroyShip();
        }
    }

    protected override void Update()
    {
        base.Update();
        UpdateShoot();
    }

    #endregion

    #region Game Logic Functions

    private void UpdateShoot()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer > enemyData.maxShootTimer)
        {
            Shoot();
            shootTimer = 0.0f;
        }
    }

    protected override void UpdatePhysicsMovement()
    {
        base.UpdatePhysicsMovement();

        rigidBody.AddForce(moveDirection * speed);

        if (rigidBody.velocity.magnitude > speed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * speed;
        }
    }

    protected void DestroyShip()
    {
        ReturnObject();
    }

    private void Shoot()
    {
        Vector3 randomDirection = ExtensionUtility.GetRandomAngleDirection(8);

        GameObject bulletObj = ObjectPoolManager.Instance.GetPooledObject(EObjectPooling.Bullet);
        bulletObj.transform.position = transform.position + randomDirection * 0.5f;
        Bullet bulletComp = bulletObj.GetComponent<Bullet>();
        bulletComp.SetBulletVariables(randomDirection, LayerMask.NameToLayer(StaticDefines.LAYER_PLAYER), LayerMask.NameToLayer(StaticDefines.LAYER_ENEMY), unitData.shootingSpeed);
    }

    #endregion
}