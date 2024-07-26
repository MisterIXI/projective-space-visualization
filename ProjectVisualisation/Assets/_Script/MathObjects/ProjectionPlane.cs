using System;
using UnityEngine;

public class ProjectionPlane : Plane
{
    public const int MESH_VERT_COUNT_X = 250;
    public event Action<ProjectionPlane> OnProjectionPlaneUpdated;
    public bool RepresentAsPlane = true;
    private float _stateT = 1f;
    public bool IsAnimating => _stateT != 1 && _stateT != 0;
    private float _animLength => SettingsManager.EnvironmentDefaults.AnimationLength;
    public float AnimationLength = 1f;
    private Mesh _mesh;

    private Vector3[] _planeVerts;
    private Vector3[] _hemiVerts;

    public static ProjectionPlane CreateProjectionPlane(Vector3 origin, Vector3 normalVector, string name = "ProjectionPlane", float size = 100f, Color? color = null, bool isCentered = true, float offset = 0f, bool representAsPlane = true, Material material = null)
    {
        ProjectionPlane planeObj = new GameObject(name).AddComponent<ProjectionPlane>();
        planeObj._meshFilter = planeObj.gameObject.AddComponent<MeshFilter>();
        planeObj._meshRenderer = planeObj.gameObject.AddComponent<MeshRenderer>();
        planeObj._meshRenderer.material = SettingsManager.EnvironmentDefaults.TransparentMaterial;
        planeObj.InsertPlaneValues(origin, normalVector, size, color ?? Color.white, isCentered, offset, material);
        planeObj.RepresentAsPlane = representAsPlane;
        planeObj._stateT = representAsPlane ? 1 : 0;
        planeObj._mesh = planeObj.CreateMesh();
        planeObj.UpdateMesh();
        return planeObj;
    }
    private void Update()
    {
        if ((RepresentAsPlane && _stateT != 1) || (!RepresentAsPlane && _stateT != 0))
        {
            _stateT = Mathf.Clamp01(_stateT + (RepresentAsPlane ? 1 : -1) * Time.deltaTime / AnimationLength);
            UpdateMesh();
            OnProjectionPlaneUpdated?.Invoke(this);
        }
    }
    private Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        Vector2[] uv = new Vector2[MESH_VERT_COUNT_X * MESH_VERT_COUNT_X];
        _planeVerts = new Vector3[MESH_VERT_COUNT_X * MESH_VERT_COUNT_X];
        _hemiVerts = new Vector3[MESH_VERT_COUNT_X * MESH_VERT_COUNT_X];
        float offset = _size / 2;
        float radius = 1f;
        for (int i = 0; i < MESH_VERT_COUNT_X; i++)
        {
            for (int j = 0; j < MESH_VERT_COUNT_X; j++)
            {
                Vector3 planeVertex = new Vector3((float)i / MESH_VERT_COUNT_X * _size - offset, _offset, (float)j / MESH_VERT_COUNT_X * _size - offset);
                // Map to hemisphere
                float phi = Mathf.PI * 2f * ((float)i / (MESH_VERT_COUNT_X - 1)); // Azimuthal angle
                float theta = Mathf.PI * ((float)j / MESH_VERT_COUNT_X / 2f); // Polar angle

                Vector3 hemisphereVertex = new Vector3(
                    radius * Mathf.Sin(theta) * Mathf.Cos(phi),
                    radius * Mathf.Cos(theta),
                    radius * Mathf.Sin(theta) * Mathf.Sin(phi)
                );
                hemisphereVertex -= _normalVector * _offset;

                _planeVerts[i * MESH_VERT_COUNT_X + j] = planeVertex;
                _hemiVerts[i * MESH_VERT_COUNT_X + j] = hemisphereVertex;
                uv[i * MESH_VERT_COUNT_X + j] = new Vector2(i / (float)MESH_VERT_COUNT_X, j / (float)MESH_VERT_COUNT_X);
            }
        }
        mesh.vertices = _planeVerts;
        int[] triangles = new int[(MESH_VERT_COUNT_X - 1) * (MESH_VERT_COUNT_X - 1) * 2 * 3];
        for (int i = 0; i < MESH_VERT_COUNT_X - 1; i++)
        {
            for (int j = 0; j < MESH_VERT_COUNT_X - 1; j++)
            {
                int index = (i * (MESH_VERT_COUNT_X - 1) + j) * 6;
                triangles[index] = i * MESH_VERT_COUNT_X + j;
                triangles[index + 1] = i * MESH_VERT_COUNT_X + j + 1;
                triangles[index + 2] = (i + 1) * MESH_VERT_COUNT_X + j;
                triangles[index + 3] = i * MESH_VERT_COUNT_X + j + 1;
                triangles[index + 4] = (i + 1) * MESH_VERT_COUNT_X + j + 1;
                triangles[index + 5] = (i + 1) * MESH_VERT_COUNT_X + j;
            }
        }
        mesh.triangles = triangles;
        Vector3[] normals = new Vector3[MESH_VERT_COUNT_X * MESH_VERT_COUNT_X];
        for (int i = 0; i < MESH_VERT_COUNT_X; i++)
        {
            for (int j = 0; j < MESH_VERT_COUNT_X; j++)
            {
                normals[i * MESH_VERT_COUNT_X + j] = Vector3.up;
            }
        }
        mesh.normals = normals;
        return mesh;
    }
    private void UpdateMesh()
    {
        // lerp between halfsphere and plane
        Vector3[] vertices = _mesh.vertices;
        Vector3[] normals = _mesh.normals;
        Vector3 originPoint = transform.position + _normalVector * -_offset;
        for (int i = 0; i < MESH_VERT_COUNT_X; i++)
        {
            for (int j = 0; j < MESH_VERT_COUNT_X; j++)
            {
                int index = i * MESH_VERT_COUNT_X + j;
                vertices[index] = Vector3.Lerp(_planeVerts[index].normalized, _planeVerts[index], _stateT);
                // vertices[index] = Vector3.Lerp(_hemiVerts[index], _planeVerts[index], _stateT);
            }
        }
        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();
        _meshFilter.mesh = _mesh;
    }
    protected override void ScaleMesh()
    {
        // check for first start
        if (_planeVerts == null)
            return;
        float offset = _size / 2;
        for (int i = 0; i < MESH_VERT_COUNT_X; i++)
        {
            for (int j = 0; j < MESH_VERT_COUNT_X; j++)
            {
                Vector3 planeVertex = new Vector3((float)i / MESH_VERT_COUNT_X * _size - offset, _offset, (float)j / MESH_VERT_COUNT_X * _size - offset);
                _planeVerts[i * MESH_VERT_COUNT_X + j] = planeVertex;
            }
        }
        UpdateMesh();
    }
    public Vector3? GetIntersectionPoint(Vector3 origin, Vector3 direction)
    {
        if (direction == Vector3.zero)
        {
            Debug.LogWarning("Direction is zero!");
            return null;
        }
        float dirNormalDot = Vector3.Dot(direction, _normalVector);
        // check if line is parallel to plane
        if (Mathf.Approximately(dirNormalDot, 0))
        {
            return null;
        }
        // calculate normal intersection point
        float t = Vector3.Dot(_normalVector, _pointOnPlane - origin) / dirNormalDot;
        return origin + Mathf.Lerp(1, t, _stateT) * direction;
    }

}
