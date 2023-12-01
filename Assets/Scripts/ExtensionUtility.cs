using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

static public class ExtensionUtility
{
    static public EObjectPooling ConvertEnemyTypeToObjectType(EEnemyType enemyType)
    {
        switch (enemyType)
        {
            case EEnemyType.Asteroid:
                return EObjectPooling.Asteroid;
            case EEnemyType.FlyingSaucer:
                return EObjectPooling.PlayerShip;
            default:
                return EObjectPooling.None;
        }
    }

    static public EObjectPooling ConvertPlayerTypeToObjectType(EPlayerType playerType)
    {
        switch (playerType)
        {
            case EPlayerType.PlayerShip:
                return EObjectPooling.PlayerShip;
            default:
                return EObjectPooling.None;
        }
    }

    static public Vector3 GetRandomAngleDirection(int dividedAngles)
    {
        float angleRotation = dividedAngles != 0 ? 360.0f / dividedAngles : 0.0f;
        float randomAngle = angleRotation * Random.Range(0, dividedAngles);
        float angleInRadians = Mathf.Deg2Rad * randomAngle;

        float x = Mathf.Cos(angleInRadians);
        float y = Mathf.Sin(angleInRadians);

        return new Vector3(x, y);
    }

    static public bool TryGetInvertedOutsidePosition(Vector3 position, Vector2 offSet, out Vector3 invertedPosition)
    {
        invertedPosition = Vector3.zero;

        Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(position);
        float viewPortX = viewPortPosition.x;
        float viewPortY = viewPortPosition.y;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Convert the box size to a viewport size
        float viewportWidth = (offSet.x)  / screenWidth;
        float viewportHeight = (offSet.y) / screenHeight;

        bool outsideMinX = viewPortPosition.x < -viewportWidth;
        bool outsideMaxX = viewPortPosition.x > 1.0f + viewportWidth;
        bool outsideMinY = viewPortPosition.y < -viewportHeight;
        bool outsideMaxY = viewPortPosition.y > 1.0 + viewportHeight;

        if (outsideMinX || outsideMaxX || outsideMinY || outsideMaxY)
        {
            if (outsideMinX) { viewPortX = 1.0f + viewportWidth * 0.998f; }
            else if (outsideMaxX) { viewPortX = -viewportWidth * 0.998f; }

            if (outsideMinY) { viewPortY = 1.0f + viewportHeight * 0.998f; }
            else if (outsideMaxY) { viewPortY = -viewportHeight * 0.998f; }

            Vector2 newPos = Camera.main.ViewportToWorldPoint(new Vector2(viewPortX, viewPortY));
            invertedPosition = newPos;

            return true;
        }

        return false;
    }
}
