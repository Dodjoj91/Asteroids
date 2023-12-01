using UnityEditor;
using UnityEngine;


public class UnitData : ScriptableObject
{
    [Header("General"), Space]
    public Sprite sprite;
    public Color spriteColor = Color.white;
    public int score;
    public float shootingSpeed;

    [Header("Physics"), Space]
    public float minSpeed;
    public float maxSpeed;
}

#if UNITY_EDITOR
[CustomEditor(typeof(UnitData))]
public class UnitDataEditor : Editor
{
    private SerializedProperty sprite;
    private SerializedProperty spriteColor;
    private SerializedProperty score;
    private SerializedProperty shootingSpeed;
    private SerializedProperty minSpeed;
    private SerializedProperty maxSpeed;

    protected void SetUnitDataSerializedObjects()
    {
        sprite = serializedObject.FindProperty("sprite");
        spriteColor = serializedObject.FindProperty("spriteColor");
        score = serializedObject.FindProperty("score");
        shootingSpeed = serializedObject.FindProperty("shootingSpeed");
        minSpeed = serializedObject.FindProperty("minSpeed");
        maxSpeed = serializedObject.FindProperty("maxSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(sprite);
        EditorGUILayout.PropertyField(spriteColor);
        EditorGUILayout.PropertyField(score);
        EditorGUILayout.PropertyField(shootingSpeed);
        EditorGUILayout.PropertyField(minSpeed);
        EditorGUILayout.PropertyField(maxSpeed);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif