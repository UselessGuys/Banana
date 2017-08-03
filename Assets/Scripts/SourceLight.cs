using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Networking;

public class SourceLight : MonoBehaviour
{
    public float Range;
    public Color Color;
    public Material Material;
    public LayerMask LayerMask;
    public int Inc;
    public float Intensity;

    private Mesh _mesh;
    private MeshRenderer _renderer;
    private Light _light;
    private Vector3[] _vertices;
    private MeshFilter _filter;

    private static Vector3 PointOnCircle(float radius, float angleInDegrees, Vector2 center)
    {
        float x = (float)(radius * Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)) + center.x;
        float y = (float)(radius * Mathf.Sin(angleInDegrees * Mathf.Deg2Rad)) + center.y;

        return new Vector3(x, y);
    }


    void Start()
    {
        _vertices = new Vector3[360/Inc];

        gameObject.AddComponent(typeof(MeshFilter));
        _filter = gameObject.GetComponent<MeshFilter>();

        gameObject.AddComponent(typeof(Light));
        _light = gameObject.GetComponent<Light>();

        gameObject.AddComponent(typeof(MeshRenderer));
        _renderer = GetComponent<MeshRenderer>();

        _mesh = new Mesh();

    }

    private void GenMesh()
    {
        for (int i = 0; i < 360 / Inc; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(this.transform.position, PointOnCircle(Range, i * Inc, this.transform.position) - this.transform.position, Range, LayerMask);

            float R = 0;
            if (hit.collider != null && hit.distance <= Range)
            {
                R = hit.distance;
            }
            else
            {
                R = Range;
            }

            _vertices[i] = (PointOnCircle(R, i * Inc, this.transform.position) - this.transform.position);
        }

        Triangulator tr = new Triangulator(_vertices);
        int[] indices = tr.Triangulate();


        _mesh.vertices = _vertices;
        Vector2[] uv = new Vector2[_vertices.Length];
        for (int i = 0; i < _vertices.Length; i++)
        {
            uv[i] = new Vector2((_vertices[i].x / Range / 2) + 0.5f, (_vertices[i].y / Range / 2) + 0.5f);
        }
        _mesh.uv = uv;
        _mesh.triangles = indices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _filter.mesh = _mesh;
    }
    void Update()
    {

        _light.color = Color;
        Material.color = Color;
       _renderer.sharedMaterial = Material;
 
    }

    void FixedUpdate()
    {
        GenMesh();
    }
}
