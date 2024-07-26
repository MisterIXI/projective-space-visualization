using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Line : MonoBehaviour
{
    private Vector3 _dirVector => _direction - _origin;
    [field: SerializeField] private Vector3 _origin;
    [field: SerializeField] private Vector3 _direction;
    [field: SerializeField] private float _length;
    [field: SerializeField] private bool _isCentered;
    [field: SerializeField] private float _width;
    [field: SerializeField] private Color _color;
    public Vector3 Origin => _origin;
    public Vector3 Direction => _direction;
    public Transform OriginTransform => transform.parent;
    private MeshRenderer _meshRenderer;
    private bool _intersectionsVisible;
    private Dictionary<ProjectionPlane, Transform> _intersections = new Dictionary<ProjectionPlane, Transform>();
    public static Line CreateLine(Vector3 origin, Vector3 direction, string name = "Line", float length = 1000f, Color? color = null, float width = 0.01f, bool isCentered = true, Material material = null, bool intersectionsVisible = true)
    {
        GameObject parent = new GameObject(name);
        Line lineObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder).AddComponent<Line>();
        lineObj.transform.parent = parent.transform;
        lineObj.gameObject.name = name + ":cylinder";
        lineObj._origin = origin;
        lineObj._direction = direction;
        lineObj._length = length;
        lineObj._isCentered = isCentered;
        lineObj._width = width;
        lineObj._color = color ?? Color.white;
        lineObj._meshRenderer = lineObj.GetComponent<MeshRenderer>();
        material = material ?? SettingsManager.EnvironmentDefaults.
        TransparentMaterial;
        lineObj._intersectionsVisible = intersectionsVisible;
        lineObj._meshRenderer.material = material;
        lineObj.UpdateLine();
        return lineObj;
    }

    private void UpdateIntersections()
    {
        foreach (KeyValuePair<ProjectionPlane, Transform> intersection in _intersections)
        {
            UpdateIntersection(intersection.Key);
        }
    }

    private void UpdateIntersection(ProjectionPlane plane)
    {
        if (!_intersections.ContainsKey(plane))
        {
            Debug.LogWarning("Line does not have intersection with this plane");
            return;
        }
        Transform intersection = _intersections[plane];
        Vector3? intersectionPoint = plane.GetIntersectionPoint(Origin, _dirVector.normalized);
        if (intersectionPoint == null)
        {
            intersection.gameObject.SetActive(false);
        }
        else
        {
            intersection.gameObject.SetActive(true);
            intersection.localPosition = intersectionPoint.Value;
        }
        intersection.gameObject.SetActive(_intersectionsVisible);
    }

    public void AddIntersection(ProjectionPlane plane)
    {
        if (_intersections.ContainsKey(plane))
        {
            Debug.LogWarning("Line already has intersection with this plane");
            return;
        }
        Transform intersection = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        intersection.GetComponent<MeshRenderer>().material = SettingsManager.EnvironmentDefaults.OpaqueMaterial;
        intersection.GetComponent<MeshRenderer>().material.color = Color.black;
        intersection.name = "Intersection:" + name + "<->" + plane.name;
        intersection.parent = transform.parent;
        intersection.localScale = Vector3.one * 0.5f;
        _intersections.Add(plane, intersection);
        plane.OnProjectionPlaneUpdated += UpdateIntersection;
        UpdateIntersection(plane);
        UpdateLine();
    }

    public void RemoveIntersection(ProjectionPlane plane)
    {
        if (!_intersections.ContainsKey(plane))
        {
            Debug.LogWarning("Line does not have intersection with this plane");
            return;
        }
        Transform intersection = _intersections[plane];
        _intersections.Remove(plane);
        plane.OnProjectionPlaneUpdated -= UpdateIntersection;
        Destroy(intersection.gameObject);
    }

    public void UpdateLineValues(Vector3? origin = null, Vector3? direction = null, float? length = null, Color? color = null, float? width = null, bool? isCentered = null, string name = null, Material material = null, bool? intersectionsVisible = null)
    {
        _origin = origin ?? _origin;
        _direction = direction ?? _direction;
        _length = length ?? _length;
        _color = color ?? _color;
        _width = width ?? _width;
        _isCentered = isCentered ?? _isCentered;
        gameObject.name = name ?? gameObject.name;
        _meshRenderer.material = material ?? _meshRenderer.material;
        _intersectionsVisible = intersectionsVisible ?? _intersectionsVisible;
        UpdateIntersections();
        UpdateLine();
    }
    public void UpdateLine()
    {
        transform.position = _origin;
        if (_dirVector != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(Vector3.Cross(_dirVector, Vector3.up), _dirVector);
        if (_length == 0)
            _length = 1000;
        if (!_isCentered)
            transform.position += transform.up * _length / 2;
        transform.localScale = new Vector3(_width, _length / 2, _width);
        GetComponent<MeshRenderer>().material.color = _color;
        foreach (Transform intersection in _intersections.Values)
        {
            intersection.localScale = Vector3.one * _width * 4;
        }
    }
    private void OnValidate()
    {
        if (Application.isPlaying)
            UpdateLine();
    }

    private void OnDestroy()
    {
        // Remove all intersections
        foreach (ProjectionPlane plane in _intersections.Keys.ToList())
        {
            RemoveIntersection(plane);
        }
        Destroy(transform.parent.gameObject);
    }

}