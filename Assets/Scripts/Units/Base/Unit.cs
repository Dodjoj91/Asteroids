using UnityEngine;

public class Unit : MonoBehaviour, IUnit
{
    #region Variables

    protected UnitData unitData;
    protected Rigidbody2D rigidBody;
    protected BoxCollider2D boxCollider2d;
    protected SpriteRenderer spriteRenderer;

    protected float speed = 1.0f;
    Vector2 boxSize;

    #endregion

    #region Properties

    public UnitData UnitData { get { return unitData; } set { unitData = value; } }

    protected UnitDataPlayer UnitDataPlayer 
    {
        get
        {
            if (unitData is UnitDataPlayer) { return unitData as UnitDataPlayer; }
            return null;
        }
    }

    protected UnitDataEnemy UnitDataEnemy
    {
        get
        {
            if (unitData is UnitDataEnemy) { return unitData as UnitDataEnemy; }
            return null;
        }
    }

    #endregion

    #region Unity Functions

    protected virtual void Start()
    {
        if (rigidBody == null) { rigidBody = GetComponent<Rigidbody2D>(); }
        if (boxCollider2d == null) { boxCollider2d = GetComponent<BoxCollider2D>(); }
        if (spriteRenderer == null) { spriteRenderer = GetComponent<SpriteRenderer>(); }

        boxSize = boxCollider2d.size;

        spriteRenderer.sprite = unitData.sprite;
        spriteRenderer.color = unitData.spriteColor;

        speed = Random.Range(unitData.minSpeed, unitData.maxSpeed);
    }

    protected virtual void Update()
    {
        UpdateMovement();
        TeleportOutsideBoundaries();
    }

    protected virtual void FixedUpdate()
    {
        UpdatePhysicsMovement();
    }

    #endregion

    #region Interface

    public void Hit()
    {
        GameObject explosion = ObjectPoolManager.Instance.GetPooledObject(EObjectPooling.ExplosionParticle);
        explosion.transform.position = transform.position;
        AddScore();
    }

    public void ReturnObject()
    {
        if (unitData != null)
        {
            if (unitData is UnitDataEnemy)
            {
                UnitDataEnemy unitEnemyData = unitData as UnitDataEnemy;
                ManagerSystem.Instance.GameManager.RemoveEnemy(unitEnemyData.enemyType, gameObject);
                ObjectPoolManager.Instance.ReturnObject(ExtensionUtility.ConvertEnemyTypeToObjectType(unitEnemyData.enemyType), gameObject);
            }
            else if (unitData is UnitDataPlayer)
            {
                UnitDataPlayer unitPlayerData = unitData as UnitDataPlayer;
                ObjectPoolManager.Instance.ReturnObject(ExtensionUtility.ConvertPlayerTypeToObjectType(unitPlayerData.playerType), gameObject);
            }
        }
    }

    #endregion

    #region Game Logic Functions

    virtual protected void UpdateMovement() { }

    virtual protected void UpdatePhysicsMovement() { }

    protected void AddScore()
    {
        ManagerSystem.Instance.GameManager.AddScore(UnitData.score);
    }

    private void TeleportOutsideBoundaries()
    {
        if (ExtensionUtility.TryGetInvertedOutsidePosition(transform.position, boxSize * 10.0f, out Vector3 invertedPosition))
        {
            transform.position = invertedPosition;
        }
    }

    #endregion
}
