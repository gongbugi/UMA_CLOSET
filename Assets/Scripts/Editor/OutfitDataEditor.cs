using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OutfitData))]
public class OutfitDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 타겟 객체를 OutfitData 타입으로 가져옴
        OutfitData data = (OutfitData)target;

        // 변경 사항을 기록 시작
        serializedObject.Update();

        // 공통 필드 표시
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outfitName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("thumbnail"));

        EditorGUILayout.Space();

        // UMA 레시피 필드 표시
        EditorGUILayout.LabelField("UMA Recipe", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("slotName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("overlayName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("raceName"));

        EditorGUILayout.Space();

        // 카테고리 필드 표시 (조건부)
        EditorGUILayout.LabelField("Categories", EditorStyles.boldLabel);

        // type 값에 따라 다른 필드를 보여줌
        switch (data.type)
        {
            case ClothingType.Top:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("topCategory"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sleeveType"));
                break;

            case ClothingType.Bottom:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bottomCategory"));
                break;
        }

        // 변경 사항 적용
        serializedObject.ApplyModifiedProperties();
    }
}
