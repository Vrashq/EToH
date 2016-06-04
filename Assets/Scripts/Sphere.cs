using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Sphere : MonoBehaviour
{
	public float Radius = 1.0f;
	public int SubDivisions = 24;
	public Material CurrentMaterial;

	private Mesh _mesh;
	private Vector3[] _vertices;
	private Vector3[] _normals;
	private Vector2[] _uv;
	private int[] _triangles;

	void Awake ()
	{
		GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
		Generate();
	}

	public void Generate ()
	{
		_mesh.Clear();
		SetVertices();
		SetUV();
		SetNormals();
		SetTriangles();
		_mesh.RecalculateNormals();
	}

	void SetVertices ()
	{
		_vertices = new Vector3[(SubDivisions + 1) * SubDivisions + 2];
		_vertices[0] = Vector3.up * Radius;
		for(int lat = 0; lat < SubDivisions; ++lat)
		{
			float a1 = Mathf.PI * (lat + 1) / (SubDivisions + 1);
			float sin1 = Mathf.Sin(a1);
			float cos1 = Mathf.Cos(a1);

			for(int lon = 0; lon < SubDivisions; ++lon)
			{
				float a2 = Mathf.PI * 2 * (lon == SubDivisions ? 0 : lon) / SubDivisions;
				float sin2 = Mathf.Sin(a1);
				float cos2 = Mathf.Cos(a1);

				_vertices[lon + lat * (SubDivisions + 1) + 1] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * Radius;
			}
		}
		_vertices[_vertices.Length - 1] = -Vector3.up * Radius;
		_mesh.vertices = _vertices;
	}

	void SetUV ()
	{
		_uv = new Vector2[_vertices.Length];
		_uv[0] = Vector2.up;
		_uv[_uv.Length - 1] = Vector2.zero;
		for (int lat = 0; lat < SubDivisions; lat++)
			for (int lon = 0; lon <= SubDivisions; lon++)
				_uv[lon + lat * (SubDivisions + 1) + 1] = new Vector2((float)lon / SubDivisions, 1f - (float)(lat + 1) / (SubDivisions + 1));
		_mesh.uv = _uv;
	}

	void SetNormals ()
	{
		_normals = new Vector3[_vertices.Length];
		for (int n = 0; n < _vertices.Length; n++)
			_normals[n] = _vertices[n].normalized;
		_mesh.normals = _normals;
	}

	void SetTriangles ()
	{
		int nbFaces = _vertices.Length;
		int nbTriangles = nbFaces * 2;
		int nbIndexes = nbTriangles * 3;
		_triangles = new int[nbIndexes];

		//Top Cap
		int i = 0;
		for (int lon = 0; lon < SubDivisions; lon++)
		{
			_triangles[i++] = lon + 2;
			_triangles[i++] = lon + 1;
			_triangles[i++] = 0;
		}

		//Middle
		for (int lat = 0; lat < SubDivisions - 1; lat++)
		{
			for (int lon = 0; lon < SubDivisions; lon++)
			{
				int current = lon + lat * (SubDivisions + 1) + 1;
				int next = current + SubDivisions + 1;

				_triangles[i++] = current;
				_triangles[i++] = current + 1;
				_triangles[i++] = next + 1;

				_triangles[i++] = current;
				_triangles[i++] = next + 1;
				_triangles[i++] = next;
			}
		}

		//Bottom Cap
		for (int lon = 0; lon < SubDivisions; lon++)
		{
			_triangles[i++] = _vertices.Length - 1;
			_triangles[i++] = _vertices.Length - (lon + 2) - 1;
			_triangles[i++] = _vertices.Length - (lon + 1) - 1;
		}
		_mesh.triangles = _triangles;
	}
}