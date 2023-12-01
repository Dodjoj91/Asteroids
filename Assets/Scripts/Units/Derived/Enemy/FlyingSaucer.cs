using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlyingSaucer : Unit
{
    UnitDataEnemy enemyData;

    Vector2 direction;
    float shootTimer = 0.0f;


    protected override void Start()
    {
        base.Start();
        enemyData = UnitDataEnemy;
        direction = ExtensionUtility.GetRandomAngleDirection(8);
    }


    protected override void Update()
    {
        base.Update();
        rigidBody.AddForce(direction * speed);

        if (rigidBody.velocity.magnitude > speed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * speed;
        }

        shootTimer += Time.deltaTime;

        if (shootTimer > enemyData.maxShootTimer)
        {
            Shoot();
            shootTimer = 0.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticDefines.TAG_BULLET))
        {
            Hit();
            DestroyShip();
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
}
