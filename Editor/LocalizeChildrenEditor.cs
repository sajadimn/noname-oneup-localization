using UnityEditor;

public abstract class LocalizeChildrenEditor : Editor
{
    private const string ScriptName = "m_Script";
    private const string ToggleName = "switchCostumeChildren";
    private const string ChildrenName = "children";

    public static bool ShowChildren(SerializedObject target)
    {
        SerializedProperty toggle = target.FindProperty(ToggleName);
        return toggle != null && toggle.boolValue;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        using (new EditorGUI.DisabledScope(true))
            EditorGUILayout.PropertyField(serializedObject.FindProperty(ScriptName));

        EditorGUILayout.PropertyField(serializedObject.FindProperty(ToggleName));

        if (ShowChildren(serializedObject))
            EditorGUILayout.PropertyField(serializedObject.FindProperty(ChildrenName), true);

        serializedObject.ApplyModifiedProperties();
    }
}
