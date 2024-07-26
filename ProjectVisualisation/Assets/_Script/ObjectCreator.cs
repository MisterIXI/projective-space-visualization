using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ObjectCreator : MonoBehaviour
{
    [HideInInspector] public bool GeneratorRunning = false;
    [HideInInspector] public float GeneratorDelay = 0.3f;
    [HideInInspector] public float Spread = 1f;
    private List<Line> _lines = new List<Line>();
    private List<ProjectionPlane> _projectionPlanes = new List<ProjectionPlane>();
    public int PlaneCount => _projectionPlanes.Count;
    public int LineCount => _lines.Count;
    public List<ProjectionPlane> PlaneList => _projectionPlanes;
    [HideInInspector] public bool LinesVisible = true;

    public void TogglePlaneRepresentation()
    {
        if (_projectionPlanes.Count == 0)
        {
            Debug.LogWarning("No projection planes created");
            return;
        }
        ProjectionPlane plane = _projectionPlanes.First();
        plane.RepresentAsPlane = !plane.RepresentAsPlane;
    }
    public void CreatePlaneY1(float size)
    {
        if (_projectionPlanes.Count > 0)
        {
            Debug.LogWarning("Plane already created");
            return;
        }
        ProjectionPlane plane = ProjectionPlane.CreateProjectionPlane(
            origin: new Vector3(1f, 0, 1f),
            normalVector: Vector3.up,
            name: "PlaneY1",
            color: new Color(0.5f, 0.5f, 0.5f, 0.5f),
            isCentered: true,
            offset: 1f,
            size: size,
            material: SettingsManager.EnvironmentDefaults.TransparentMaterial
        );
        _projectionPlanes.Add(plane);
    }
    // read float from input field

    public void AddRandomY1IntersectingLine()
    {
        if (_projectionPlanes.Count == 0)
        {
            Debug.LogWarning("No projection planes created");
            return;
        }
        StartCoroutine(RandomIntersectGenerator());
        GeneratorRunning = true;
    }

    private IEnumerator RandomIntersectGenerator()
    {
        while (true)
        {
            Line line = Line.CreateLine(
            origin: new Vector3(1f, 0, 1f),
            direction: new Vector3(1f + Random.Range(-Spread, Spread), 1, 1f + Random.Range(-Spread, Spread)),
            name: "IntersectingLine",
            color: Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1, 1, 1),
            material: SettingsManager.EnvironmentDefaults.OpaqueMaterial);
            _lines.Add(line);
            line.AddIntersection(_projectionPlanes.First());
            line.GetComponent<MeshRenderer>().enabled = LinesVisible;
            yield return new WaitForSeconds(GeneratorDelay);
        }
    }

    public void AddRandomNonIntersectingLine()
    {
        StartCoroutine(NoIntersectGenerator());
        GeneratorRunning = true;
    }
    public void ToggleLineVisibility()
    {
        LinesVisible = !LinesVisible;
        foreach (Line line in _lines)
        {
            line.GetComponent<MeshRenderer>().enabled = LinesVisible;
        }
    }
    private IEnumerator NoIntersectGenerator()
    {
        while (true)
        {
            Line line = Line.CreateLine(
                origin: new Vector3(1f, 0, 1f),
                direction: new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized,
                name: "NonIntersectingLine",
                color: Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1, 1, 1),
                material: SettingsManager.EnvironmentDefaults.OpaqueMaterial);
            _lines.Add(line);
            yield return new WaitForSeconds(GeneratorDelay);
        }
    }

    public void ClearAllObjects()
    {
        foreach (Line line in _lines)
        {
            Destroy(line.gameObject);
        }
        _lines.Clear();
        foreach (ProjectionPlane plane in _projectionPlanes)
        {
            Destroy(plane.gameObject);
        }
        _projectionPlanes.Clear();
    }
}