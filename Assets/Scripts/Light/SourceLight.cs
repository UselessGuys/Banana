using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SourceLight : MonoBehaviour
{
    public float Range = 1;
    public Color Color;
    public Material Material;
    public Texture2D Texture;
    public LayerMask LayerMask;
    public int Inc = 6;
    public float Z = 0;

    private Mesh _mesh;
    public Vector2[] _uv;
    private MeshRenderer _renderer;
    private int[] triangles;
    private Vector3[] _vertices;
    private MeshFilter _filter;

    private static Vector3 PointOnCircle(float radius, float angleInDegrees, Vector2 center)
    {
        var x = (float)(radius * Mathf.Cos(angleInDegrees * Mathf.Deg2Rad)) + center.x;
        var y = (float)(radius * Mathf.Sin(angleInDegrees * Mathf.Deg2Rad)) + center.y;

        return new Vector3(x, y);
    }


    private void Start()
    {
        _vertices = new Vector3[(360 / Inc) + 1];

        gameObject.AddComponent(typeof(MeshFilter));
        _filter = gameObject.GetComponent<MeshFilter>();
        
        gameObject.AddComponent(typeof(MeshRenderer));
        _renderer = GetComponent<MeshRenderer>();

        _mesh = new Mesh();


        Material = Instantiate(Material) as Material;
    }

    private void GenMesh()
    {

        _vertices[0] = new Vector3(0, 0,-this.transform.position.z);
        for (var i = 0; i < (360 / Inc); i++)
        {
            var hit = Physics2D.Raycast(transform.position, PointOnCircle(Range, i * Inc, transform.position) - transform.position, Range, LayerMask);

            float R = 0;
            if (hit.collider != null && hit.distance <= Range)
                R = hit.distance;
            else
                R = Range;

            _vertices[i+1] = PointOnCircle(R, i * Inc, transform.position) - transform.position;
        }

        triangles = new int[360 / Inc *3];

        for (int i = 0; i < (360 / Inc )-1; i++)
        {
            triangles[i * 3] = 0;
            triangles[(i * 3) + 1] = i + 1;
            triangles[(i * 3) + 2] = i + 2;
        }
        triangles[((360 / Inc) - 1) * 3] = 0;
        triangles[(((360 / Inc) - 1) * 3) + 1] = 360 / Inc;
        triangles[(((360 / Inc) - 1) * 3) + 2] = 1;
        _uv = new Vector2[(360 / Inc)+1];
        for (var i = 0; i < (360 / Inc)+1; i++)
            _uv[i] = new Vector2(_vertices[i].x / Range / 2 + 0.5f, _vertices[i].y / Range / 2 + 0.5f);

        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.uv = _uv;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    
        _filter.mesh = _mesh;

        Material.color = Color;
        Material.mainTexture = Texture;

        _renderer.sharedMaterial = Material;
    }

    private void Update()
    {

        GenMesh();


    }

    private void FixedUpdate()
    {
        
    }


}
