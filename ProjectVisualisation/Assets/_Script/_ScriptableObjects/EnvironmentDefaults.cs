using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentDefaults", menuName = "ProjectVisualisation/EnvironmentDefaults", order = 0)]
public class EnvironmentDefaults : ScriptableObject

{
    [field: Header("Animations")]
    [field: SerializeField][field: Range(0.01f, 5f)] public float AnimationLength { get; private set; } = 1f;
    [field: Header("Materials")]
    [field: SerializeField] public Material TransparentMaterial { get; private set; }
    [field: SerializeField] public Material OpaqueMaterial { get; private set; }
    [field: SerializeField] public Material YColoredMaterial { get; private set; }
    [field: Header("Grid")]
    [field: SerializeField] public bool CenterGrid { get; private set; } = true;
    [field: SerializeField][field: Range(1, 200)] public float PlaneSize { get; private set; } = 100;
    [field: SerializeField][field: Range(1, 200)] public float AxisLength { get; private set; } = 100;
    [field: SerializeField] public Color XPlaneColor { get; private set; } = new Color(1, 0, 0, 0.5f);
    [field: SerializeField] public Color YPlaneColor { get; private set; } = new Color(0, 1, 0, 0.5f);
    [field: SerializeField] public Color ZPlaneColor { get; private set; } = new Color(0, 0, 1, 0.5f);

    [field: Header("Overrides")]
    [field: SerializeField][field: Range(0, 1)] public float TransparencyOverride { get; private set; } = 0.5f;
    

    public void OverrideTransparency()
    {
        XPlaneColor = new Color(XPlaneColor.r, XPlaneColor.g, XPlaneColor.b, TransparencyOverride);
        YPlaneColor = new Color(YPlaneColor.r, YPlaneColor.g, YPlaneColor.b, TransparencyOverride);
        ZPlaneColor = new Color(ZPlaneColor.r, ZPlaneColor.g, ZPlaneColor.b, TransparencyOverride);
        OnEnvironmentDefaultsChanged?.Invoke();
    }

    public static Action OnEnvironmentDefaultsChanged;

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            OnEnvironmentDefaultsChanged?.Invoke();
        }
    }
}