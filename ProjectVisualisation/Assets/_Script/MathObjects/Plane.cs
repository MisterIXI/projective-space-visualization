using System;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public event Action OnPlaneUpdated;
    protected Vector3 _pointOnPlane => _origin + _normalVector * _offset;
    [field: SerializeField] protected Vector3 _origin { get; set; }
    [field: SerializeField] protected Vector3 _normalVector { get; set; }
    [field: SerializeField] protected float _offset { get; set; }
    [field: SerializeField] protected float _size { get; set; }
    [field: SerializeField] protected Color _color { get; set; }
    [field: SerializeField] protected bool _isCentered { get; set; }
    protected MeshRenderer _meshRenderer;
    protected MeshFilter _meshFilter;
    private Mesh _mesh;
    public static Plane CreatePlane(Vector3 origin, Vector3 normalVector, string name = "Plane", float size = 1000f, Color? color = null, bool isCentered = true, float offset = 0f, Material material = null)
    {
        Plane planeObj = new GameObject().AddComponent<Plane>();
        planeObj._meshRenderer = planeObj.gameObject.AddComponent<MeshRenderer>();
        planeObj._meshFilter = planeObj.gameObject.AddComponent<MeshFilter>();
        planeObj.gameObject.name = name;
        planeObj.InsertPlaneValues(origin, normalVector, size, color ?? Color.white, isCentered, offset, material);
        planeObj.UpdatePlane();
        return planeObj;
    }

    protected void InsertPlaneValues(Vector3 origin, Vector3 normalVector, float size, Color color, bool isCentered, float offset, Material material)
    {
        if (_mesh == null)
            _mesh = new Mesh();
        _origin = origin;
        _normalVector = normalVector;
        _size = size;
        _color = color;
        _isCentered = isCentered;
        _offset = offset;
        material = material ?? SettingsManager.EnvironmentDefaults.TransparentMaterial;
        _meshRenderer.material = material;
        UpdatePlane();
    }
    public virtual void UpdatePlaneValues(Vector3? origin = null, Vector3? normalVector = null, float? size = null, Color? color = null, bool? isCentered = null, string name = null, float? offset = null, Material material = null)
    {
        _origin = origin ?? _origin;
        _normalVector = normalVector ?? _normalVector;
        _size = size ?? _size;
        _color = color ?? _color;
        _isCentered = isCentered ?? _isCentered;
        gameObject.name = name ?? gameObject.name;
        _offset = offset ?? _offset;
        _meshRenderer.material = material ?? _meshRenderer.material;
        UpdatePlane();
    }


    public virtual void UpdatePlane()
    {
        transform.position = _origin;
        if (_normalVector != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(-Vector3.Cross(_normalVector, Vector3.up), _normalVector);
        if (_size == 0)
            _size = 10;
        if (!_isCentered)
        {
            transform.position += transform.right * _size / 2;
            transform.position += transform.forward * _size / 2;
        }
        ScaleMesh();
        GetComponent<MeshRenderer>().material.color = _color;
        OnPlaneUpdated?.Invoke();
    }

    protected virtual void ScaleMesh()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-_size / 2, _offset, -_size / 2);
        vertices[1] = new Vector3(_size / 2, _offset, -_size / 2);
        vertices[2] = new Vector3(_size / 2, _offset, _size / 2);
        vertices[3] = new Vector3(-_size / 2, _offset, _size / 2);
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(0, 1);
        _mesh.vertices = vertices;
        _mesh.uv = uvs;
        _mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        _mesh.RecalculateNormals();
        _meshFilter.mesh = _mesh;
    }
}