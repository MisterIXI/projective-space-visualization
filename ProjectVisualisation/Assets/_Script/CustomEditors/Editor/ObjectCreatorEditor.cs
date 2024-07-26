using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectCreator))]
public class ObjectCreatorEditor : Editor
{
    private float spread = 12.5f;
    private float generatorDelay = 0.3f;
    private int size = 25;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ObjectCreator objectCreator = (ObjectCreator)target;
        size = EditorGUILayout.IntSlider("Plane Size", size, 5, int.MaxValue-1);
        if (objectCreator.PlaneCount == 0)
        {
            if (GUILayout.Button("Create Plane Y1") && Application.isPlaying)
            {
                objectCreator.CreatePlaneY1(size);
            }
        }
        else
        {
            if (GUILayout.Button("Clear all and Update Plane Y1") && Application.isPlaying)
            {
                objectCreator.ClearAllObjects();
                objectCreator.CreatePlaneY1(size);
            }
        }
        if (objectCreator.PlaneList.Count > 0)
        {
            float animSpeed = 1f;
            animSpeed = objectCreator.PlaneList.First().AnimationLength;
            animSpeed = EditorGUILayout.Slider("Animation Length", animSpeed, 0.01f, 5f);
            objectCreator.PlaneList.First().AnimationLength = animSpeed;
        }
        if (GUILayout.Button("Toggle Plane Representation") && Application.isPlaying)
        {
            objectCreator.TogglePlaneRepresentation();
        }
        spread = EditorGUILayout.Slider("Spread", spread, 0.1f, 25f);
        objectCreator.Spread = spread;
        generatorDelay = EditorGUILayout.Slider("Generator Delay", generatorDelay, 0.01f, 1f);
        objectCreator.GeneratorDelay = generatorDelay;
        if (objectCreator.GeneratorRunning)
        {
            if (GUILayout.Button("Stop Generator") && Application.isPlaying)
            {
                objectCreator.GeneratorRunning = false;
                objectCreator.StopAllCoroutines();
            }
            if (GUILayout.Button("Stop Generator") && Application.isPlaying)
            {
                objectCreator.GeneratorRunning = false;
                objectCreator.StopAllCoroutines();
            }
        }
        else
        {
            if (GUILayout.Button("Add Random Y1 Intersecting Line") && Application.isPlaying)
            {
                objectCreator.AddRandomY1IntersectingLine();
            }
            if (GUILayout.Button("Add Random Non-Intersecting Line") && Application.isPlaying)
            {
                objectCreator.AddRandomNonIntersectingLine();
            }
        }
        if (GUILayout.Button(objectCreator.LinesVisible ? "Hide Lines" : "Show Lines") && Application.isPlaying)
        {
            objectCreator.ToggleLineVisibility();
        }
        // label that tells how many lines are currently in the scene
        EditorGUILayout.LabelField("Lines in scene: " + objectCreator.LineCount);
        if (GUILayout.Button("Clear All Objects") && Application.isPlaying)
        {
            objectCreator.ClearAllObjects();
        }
    }
}