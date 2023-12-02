using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/UnitDataEnemy")]
public class UnitDataEnemy : UnitData
{
    [Header("Enemy"), Space]
    public EEnemyType enemyType;
    public float minShootTimer = 0.8f;
    public float maxShootTimer = 1.2f;
}


#if UNITY_EDITOR
[CustomEditor(typeof(UnitDataEnemy))]
public class UnitDataEnemyEditor : UnitDataEditor
{
    private SerializedProperty enemyType;
    private SerializedProperty minShootTimer;
    private SerializedProperty maxShootTimer;

    private void OnEnable()
    {
        enemyType = serializedObject.FindProperty("enemyType");
        minShootTimer = serializedObject.FindProperty("minShootTimer");
        maxShootTimer = serializedObject.FindProperty("maxShootTimer");

        SetUnitDataSerializedObjects();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(enemyType);

        EEnemyType selectedType = (EEnemyType)enemyType.enumValueIndex;

        switch (selectedType)
        {
            case EEnemyType.Asteroid:
                break;
            case EEnemyType.FlyingSaucer:
                EditorGUILayout.PropertyField(minShootTimer);
                EditorGUILayout.PropertyField(maxShootTimer);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif