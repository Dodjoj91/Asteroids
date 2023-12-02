using UnityEngine;

[CreateAssetMenu(menuName = "Game/UnitDataPlayer")]
public class UnitDataPlayer : UnitData
{
    [Header("Player"), Space]
    public EPlayerType playerType;

    public float maxFireRateCooldown = 0.1f;
    public float maxInvincibilityTimer = 3.0f;
    public float rotationSpeed = 400.0f;
}
