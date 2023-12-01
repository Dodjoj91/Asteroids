using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IUnit
{
    [SerializeField] protected UnitData unitData;

    [SerializeField] protected Rigidbody2D rigidBody;
    [SerializeField] protected BoxCollider2D boxCollider2d;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public UnitData UnitData { get { return unitData; } set { unitData = value; } }

    Vector2 boxSize;

    protected virtual void Start()
    {
        if (rigidBody == null) { rigidBody = GetComponent<Rigidbody2D>(); }
        if (boxCollider2d == null) { boxCollider2d = GetComponent<BoxCollider2D>(); }
        if (spriteRenderer == null) { spriteRenderer = GetComponent<SpriteRenderer>(); }

        boxSize = boxCollider2d.size;

        spriteRenderer.sprite = unitData.sprite;
        spriteRenderer.color = unitData.spriteColor;
    }

    protected virtual void Update()
    {
        TeleportOutsideBoundaries();
    }

    public void Attack(int amount)
    {

    }

    public void Hit(int amount)
    {

    }

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
    bool IsOutsideViewport()
    {
        // Get the object's position in screen coordinates
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        // Calculate the bounds considering the collider's size
        float halfWidth = boxSize.x * 0.5f;
        float halfHeight = boxSize.y * 0.5f;

        // Check if the object is outside the viewport
        return (viewportPosition.x < -halfWidth || viewportPosition.x > 1 + halfWidth ||
                viewportPosition.y < -halfHeight || viewportPosition.y > 1 + halfHeight);
    }

    void TeleportToOtherSide()
    {
        // Get the main camera's position
        Vector3 cameraPosition = Camera.main.transform.position;

        // Calculate the new position on the opposite side
        Vector3 newPosition = transform.position;

        // Calculate the bounds considering the collider's size
        float halfWidth = boxSize.x * 0.5f;
        float halfHeight = boxSize.y * 0.5f;

        if (transform.position.x < Camera.main.transform.position.x)
        {
            newPosition.x = cameraPosition.x + Camera.main.orthographicSize * Camera.main.aspect + halfWidth;
        }
        else
        {
            newPosition.x = cameraPosition.x - Camera.main.orthographicSize * Camera.main.aspect - halfWidth;
        }

        newPosition.y = cameraPosition.y; // Keep the same y-position

        // Teleport the object to the new position
        transform.position = newPosition;
    }
}
