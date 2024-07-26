using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Creates the X, Y, Z planes and the axis objects.
/// </summary>
public class PlanesAndGrid : MonoBehaviour
{
    private Plane _xPlane, _yPlane, _zPlane;
    private Line _xAxis, _yAxis, _zAxis;

    private EnvironmentDefaults _envSettings => SettingsManager.EnvironmentDefaults;
    private float _axisLength => _envSettings.AxisLength;
    private float _scaledPlaneSize => _envSettings.PlaneSize / 10;
    private float _halfPlaneSize => _envSettings.PlaneSize / 2;
    private void Start()
    {
        CreatePlanes();
        CreateAxis();
        EnvironmentDefaults.OnEnvironmentDefaultsChanged += UpdateVisuals;
    }

    private void CreatePlanes()
    {
        _xPlane = Plane.CreatePlane(Vector3.zero, Vector3.left, "X Plane", _envSettings.PlaneSize, _envSettings.XPlaneColor, false);
        _yPlane = Plane.CreatePlane(Vector3.zero, Vector3.up, "Y Plane", _envSettings.PlaneSize, _envSettings.YPlaneColor, false);
        _zPlane = Plane.CreatePlane(Vector3.zero, Vector3.forward, "Z Plane", _envSettings.PlaneSize, _envSettings.ZPlaneColor, false);
        UpdatePlaneVisuals();
    }

    private void CreateAxis()
    {
        _xAxis = Line.CreateLine(Vector3.zero, Vector3.right, "X Axis", _axisLength, _envSettings.XPlaneColor, 0.01f, false, _envSettings.OpaqueMaterial);
        _yAxis = Line.CreateLine(Vector3.zero, Vector3.up, "Y Axis", _axisLength, _envSettings.YPlaneColor, 0.01f, false, _envSettings.OpaqueMaterial);
        _zAxis = Line.CreateLine(Vector3.zero, Vector3.forward, "Z Axis", _axisLength, _envSettings.ZPlaneColor, 0.01f, false, _envSettings.OpaqueMaterial);
        UpdateAxisVisuals();
    }

    private void UpdateVisuals()
    {
        UpdatePlaneVisuals();
        UpdateAxisVisuals();
    }

    private void UpdatePlaneVisuals()
    {
        _xPlane.UpdatePlaneValues(
            color: _envSettings.XPlaneColor,
            material: _envSettings.TransparentMaterial,
            size: _envSettings.PlaneSize,
            isCentered: _envSettings.CenterGrid);
        _yPlane.UpdatePlaneValues(
            color: _envSettings.YPlaneColor,
            material: _envSettings.TransparentMaterial,
            size: _envSettings.PlaneSize,
            isCentered: _envSettings.CenterGrid);
        _zPlane.UpdatePlaneValues(
            color: _envSettings.ZPlaneColor,
            material: _envSettings.TransparentMaterial,
            size: _envSettings.PlaneSize,
            isCentered: _envSettings.CenterGrid);
    }

    private void UpdateAxisVisuals()
    {
        _xAxis.UpdateLineValues(
            color: _envSettings.XPlaneColor,
            material: _envSettings.OpaqueMaterial,
            length: _axisLength,
            isCentered: _envSettings.CenterGrid);
        _yAxis.UpdateLineValues(
            color: _envSettings.YPlaneColor,
            material: _envSettings.OpaqueMaterial,
            length: _axisLength,
            isCentered: _envSettings.CenterGrid);
        _zAxis.UpdateLineValues(
            color: _envSettings.ZPlaneColor,
            material: _envSettings.OpaqueMaterial,
            length: _axisLength,
            isCentered: _envSettings.CenterGrid);
    }
}
