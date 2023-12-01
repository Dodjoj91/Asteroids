using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSaucer : Unit
{
    protected override void Start()
    {
        base.Start();
    }


    protected override void Update()
    {
        base.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticDefines.TAG_BULLET))
        {
            Hit();
        }
    }

    protected void Hit()
    {
        AddScore();
        ManagerSystem.Instance.GameManager.RemoveEnemy(EEnemyType.FlyingSaucer, gameObject);
        ObjectPoolManager.Instance.ReturnObject(EObjectPooling.FlyingSaucer, gameObject);
    }
}
