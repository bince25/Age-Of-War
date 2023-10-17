using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor
{
    SerializedProperty isFarm;
    SerializedProperty isBarrack;
    SerializedProperty isStable;
    SerializedProperty isWorkshop;

    //Farm
    SerializedProperty farmLevel;
    SerializedProperty goldIncrement;
    SerializedProperty incrementCooldown;
    SerializedProperty farmGoldMultiplier;
    SerializedProperty farmFoodMultiplier;
    SerializedProperty foodIncrement;

    //Barrack
    SerializedProperty barrackLevel;

    //Stable
    SerializedProperty stableLevel;

    //Workshop
    SerializedProperty workshopLevel;

    void OnEnable()
    {
        isFarm = serializedObject.FindProperty("isFarm");
        isBarrack = serializedObject.FindProperty("isBarrack");
        isStable = serializedObject.FindProperty("isStable");
        isWorkshop = serializedObject.FindProperty("isWorkshop");

        workshopLevel = serializedObject.FindProperty("workshopLevel");
        stableLevel = serializedObject.FindProperty("stableLevel");
        barrackLevel = serializedObject.FindProperty("barrackLevel");


        farmLevel = serializedObject.FindProperty("farmLevel");
        goldIncrement = serializedObject.FindProperty("goldIncrement");
        incrementCooldown = serializedObject.FindProperty("incrementCooldown");
        farmGoldMultiplier = serializedObject.FindProperty("farmGoldMultiplier");
        farmFoodMultiplier = serializedObject.FindProperty("farmFoodMultiplier");
        foodIncrement = serializedObject.FindProperty("foodIncrement");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(isFarm);
        EditorGUILayout.PropertyField(isBarrack);
        EditorGUILayout.PropertyField(isStable);
        EditorGUILayout.PropertyField(isWorkshop);

        if (isFarm.boolValue)
        {
            EditorGUILayout.PropertyField(farmLevel);
            EditorGUILayout.PropertyField(goldIncrement);
            EditorGUILayout.PropertyField(incrementCooldown);
            EditorGUILayout.PropertyField(farmGoldMultiplier);
            EditorGUILayout.PropertyField(farmFoodMultiplier);
            EditorGUILayout.PropertyField(foodIncrement);
        }
        else if (isBarrack.boolValue)
        {
            EditorGUILayout.PropertyField(barrackLevel);
        }
        else if (isStable.boolValue)
        {
            EditorGUILayout.PropertyField(stableLevel);
        }
        else if (isWorkshop.boolValue)
        {
            EditorGUILayout.PropertyField(workshopLevel);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
