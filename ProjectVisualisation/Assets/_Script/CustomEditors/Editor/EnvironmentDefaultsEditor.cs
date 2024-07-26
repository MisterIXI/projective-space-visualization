using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnvironmentDefaults))]
public class EnvironmentDefaultsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Override Transparency in Colors"))
        {
            var environmentDefaults = (EnvironmentDefaults)target;
            environmentDefaults.OverrideTransparency();
            EditorUtility.SetDirty(environmentDefaults);
        }
        if(GUILayout.Button("Update Visuals"))
        {
            EnvironmentDefaults.OnEnvironmentDefaultsChanged?.Invoke();
        }
    }
}