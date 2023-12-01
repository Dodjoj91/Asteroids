using UnityEngine;

public class Asteroid : Unit
{
    #region Unity Functions

    protected override void Start()
    {
        base.Start();
        SetRandomRotation(Random.Range(0.0f, 360.0f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(StaticDefines.TAG_BULLET))
        {
            Hit();
            SplitAsteroid();
        }
    }

    #endregion

    #region Game Logic Functions

    protected override void UpdatePhysicsMovement()
    {
        base.UpdatePhysicsMovement();

        rigidBody.AddForce(transform.up * speed);

        if (rigidBody.velocity.magnitude > speed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * speed;
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

    #endregion
}