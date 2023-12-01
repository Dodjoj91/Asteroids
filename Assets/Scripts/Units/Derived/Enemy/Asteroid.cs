using UnityEngine;

public class Asteroid : Unit
{
    protected override void Start()
    {
        base.Start();
        SetRandomRotation(Random.Range(0.0f, 360.0f));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        rigidBody.AddForce(transform.up * speed);

        if (rigidBody.velocity.magnitude > speed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * speed;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticDefines.TAG_BULLET))
        {
            Hit();
            SplitAsteroid();
        }
    }

    protected void SplitAsteroid()
    {
        ReturnObject();
    }

    private void SetRandomRotation(float angle)
    {
        rigidBody.MoveRotation(angle);
    }
}
