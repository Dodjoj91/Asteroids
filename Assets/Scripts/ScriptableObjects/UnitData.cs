using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("General"), Space]
    public Sprite sprite;
    public Color spriteColor = Color.white;
    public int score;

    [Header("Physics"), Space]
    public float speed;
}
