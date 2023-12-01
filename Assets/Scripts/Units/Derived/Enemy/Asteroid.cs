using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : Unit
{
    float thrust = 1.0f;

    protected override void Start()
    {
        base.Start();
        SetRandomRotation(Random.Range(0.0f, 360.0f));
        thrust = Random.Range(0.5f, 1.0f);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        rigidBody.AddForce(transform.up * thrust);

        if (rigidBody.velocity.magnitude > thrust)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * thrust;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticDefines.TAG_BULLET))
        {
            SplitAsteroid();
        }
    }

    protected void SplitAsteroid()
    {
        AddScore();
        ManagerSystem.Instance.GameManager.RemoveEnemy(EEnemyType.Asteroid, gameObject);
        ObjectPoolManager.Instance.ReturnObject(EObjectPooling.Asteroid, gameObject);
    }

    private void SetRandomRotation(float angle)
    {
        rigidBody.MoveRotation(angle);
    }
}
