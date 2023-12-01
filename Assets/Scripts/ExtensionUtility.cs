using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
