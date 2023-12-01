using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Game/UnitDataEnemy")]
public class UnitDataEnemy : UnitData
{
    [Header("Enemy"), Space]
    public EEnemyType enemyType;
    public float maxShootTimer = 1.0f;
}


[CustomEditor(typeof(UnitDataEnemy))]
public class UnitDataEnemyEditor : UnitDataEditor
{
    private SerializedProperty enemyType;
    private SerializedProperty maxShootTimer;

    private void OnEnable()
    {
        enemyType = serializedObject.FindProperty("enemyType");
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
                EditorGUILayout.PropertyField(maxShootTimer);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}