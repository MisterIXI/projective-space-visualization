using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class represents an elliptic curve of the form y^2 = x^3 + Ax + B
/// The function Evaluate(x) returns the y value for a given x value. The value can be negated to get the second y value.
/// Vector3 coordinates of given x,y values can be obtained by calling GetCoordinates(x,y).
/// The coordinates assume transform.position as the origin and transform.right as the x-axis, while transform.forward is the y-axis.
/// </summary>
public class EllipticCurve : MonoBehaviour
{
    [field: SerializeField] public float A { get; private set; } = 1;
    [field: SerializeField] public float B { get; private set; } = 1;
    [field: SerializeField] public Vector2 Range { get; private set; } = new Vector2(-5, 5);
    [field: SerializeField][field: Range(0.05f, 2f)] public float StepSize { get; private set; } = 0.1f;
    [field: SerializeField] private bool _renderGizmos = false;

    private List<Line> _lines = new List<Line>();
    private List<Line> _deactivatedLines = new List<Line>();
    private ProjectionPlane _projectionPlane;
    private bool _linesVisible = false;
    private bool _pointsVisible = true;
    private bool _specialLinesVisible = true;
    private void Start()
    {
        _projectionPlane = ProjectionPlane.CreateProjectionPlane(
            origin: new Vector3(1f, 0, 1f),
            normalVector: Vector3.up,
            name: "PlaneY1",
            color: new Color(0.5f, 0.5f, 0.5f, 0.5f),
            isCentered: true,
            offset: 1f,
            size: 25,
            material: SettingsManager.EnvironmentDefaults.TransparentMaterial
        );
        _projectionPlane.transform.parent = transform;
        _projectionPlane.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x">The x value the elliptic curve should be evaluated as</param>
    /// <returns>A resulting y value, which can be negated to get the mirrored one. Returns NaN for non existing values.</returns>
    public float Evaluate(float x)
    {
        return Mathf.Sqrt(Mathf.Pow(x, 3) + A * x + B);
    }

    public Vector3 GetCoordinates(float x, float y)
    {
        return transform.right * x + transform.forward * y;
    }

    private void UpdateLines()
    {
        if (SettingsManager.EnvironmentDefaults == null)
        {
            return;
        }
        // update number of active lines
        int lineCount = Mathf.CeilToInt((Range.y - Range.x) / StepSize * 2);
        if (!_linesVisible)
        {
            lineCount = 0;
        }
        while (_lines.Count < lineCount)
        {
            if (_deactivatedLines.Count > 0)
            {
                _lines.Add(_deactivatedLines.First());
                _deactivatedLines.RemoveAt(0);
            }
            else
            {
                Line line = Line.CreateLine(
                    origin: Vector3.zero,
                    direction: Vector3.up,
                    name: "Line",
                    color: Color.red,
                    material: SettingsManager.EnvironmentDefaults.TransparentMaterial
                );
                line.OriginTransform.parent = transform;
                line.OriginTransform.localPosition = Vector3.zero;
                line.AddIntersection(_projectionPlane);
                _lines.Add(line);
            }
        }
        while (_lines.Count > lineCount)
        {
            _deactivatedLines.Add(_lines.Last());
            _lines.Last().OriginTransform.gameObject.SetActive(false);
            _lines.RemoveAt(_lines.Count - 1);
        }
        for (int i = 0; i < _lines.Count; i += 2)
        {
            float x = Range.x + i * StepSize / 2;
            float y = Evaluate(x);
            if (!float.IsNaN(y))
            {
                _lines[i].UpdateLineValues(
                    origin: Vector3.zero,
                    direction: GetCoordinates(x, y) + Vector3.up,
                    color: new Color(0.5f, 0.5f, 0.5f, 0.5f)
                );
                _lines[i].OriginTransform.gameObject.SetActive(true);
                _lines[i + 1].UpdateLineValues(
                    origin: Vector3.zero,
                    direction: GetCoordinates(x, -y) + Vector3.up,
                    color: new Color(0.5f, 0.5f, 0.5f, 0.5f)
                );
                _lines[i + 1].OriginTransform.gameObject.SetActive(true);
            }
            else
            {
                _lines[i].OriginTransform.gameObject.SetActive(false);
                _lines[i + 1].OriginTransform.gameObject.SetActive(false);
            }
        }
    }

    public void HideAllLines()
    {
        _linesVisible = false;
        UpdateLines();
    }

    public void ShowAllLines()
    {
        _linesVisible = true;
        UpdateLines();
    }

    public void HideAllPoints()
    {
        _pointsVisible = false;
        UpdateLines();
    }

    public void ShowAllPoints()
    {
        _pointsVisible = true;
        UpdateLines();
    }

    private void FixedUpdate()
    {
        UpdateLines();
    }
    // private void OnValidate()
    // {
    //     if (Application.isPlaying)
    //     {
    //         UpdateLines();
    //     }
    // }

    private void OnDrawGizmos()
    {
        if (_renderGizmos)
        {
            Gizmos.color = Color.red;
            for (float x = Range.x; x < Range.y; x += StepSize)
            {
                float y = Evaluate(x);
                if (!float.IsNaN(y))
                {
                    Gizmos.DrawSphere(GetCoordinates(x, y) + Vector3.up, 0.1f);
                    Gizmos.DrawSphere(GetCoordinates(x, -y) + Vector3.up, 0.1f);
                }
            }
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + transform.right * Range.x, transform.position + transform.right * Range.y);
        }
    }
}