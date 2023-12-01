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
        TeleportOutsideBoundaries();
    }

    #endregion

    #region Interface

    public void Hit()
    {
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

    #region Undefined

    protected void AddScore()
    {
        ManagerSystem.Instance.GameManager.AddScore(UnitData.score);
    }

    private void TeleportOutsideBoundaries()
    {
        Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(transform.position);
        float viewPortX = viewPortPosition.x;
        float viewPortY = viewPortPosition.y;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Convert the box size to a viewport size
        float viewportWidth = (boxSize.x * 10.0f)  / screenWidth;
        float viewportHeight = (boxSize.y * 10.0f) / screenHeight;

        bool outsideMinX = viewPortPosition.x < -viewportWidth;
        bool outsideMaxX = viewPortPosition.x > 1.0f + viewportWidth;
        bool outsideMinY = viewPortPosition.y < -viewportHeight;
        bool outsideMaxY = viewPortPosition.y > 1.0 + viewportHeight;

        if (outsideMinX || outsideMaxX || outsideMinY || outsideMaxY)
        {
            if (outsideMinX) { viewPortX = 1.0f + viewportWidth * 0.998f; }
            else if (outsideMaxX) { viewPortX = -viewportWidth * 0.998f; }

            if (outsideMinY) {  viewPortY = 1.0f + viewportHeight * 0.998f; }
            else if (outsideMaxY) {  viewPortY = -viewportHeight * 0.998f; }

            Vector2 newPos = Camera.main.ViewportToWorldPoint(new Vector2(viewPortX, viewPortY));
            transform.position = newPos;
        }
    }

    #endregion
}
