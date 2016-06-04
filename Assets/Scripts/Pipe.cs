using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Pipe : MonoBehaviour
{
	public float PipeRadius;
	public int PipeSegmentCount;
	public float RingDistance;
	public float MinCurveRadius, MaxCurveRadius;
	public int MinCurveSegmentCount, MaxCurveSegmentCount;
	public PipeItemGenerator[] Generators;

	private float _curveRadius;
	private int _curveSegmentCount;
	private Mesh _mesh;
	private Vector3[] _vertices, _normals;
	private Vector2[] _uv;
	private int[] _triangles;
	private float _curveAngle;
	private float _relativeRotation;

	public float CurveAngle {
		get {
			return _curveAngle;
		}
	}

	public float CurveRadius {
		get {
			return _curveRadius;
		}
	}

	public float RelativeRotation {
		get {
			return _relativeRotation;
		}
	}

	public int CurveSegmentCount {
		get {
			return _curveSegmentCount;
		}
	}

	private void Awake () {
		GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
		_mesh.name = "Pipe";
	}

	public void Generate (bool withItems = true) {
		_curveRadius = Random.Range(MinCurveRadius, MaxCurveRadius);
		_curveSegmentCount = Random.Range(MinCurveSegmentCount, MaxCurveSegmentCount + 1);
		_mesh.Clear();
		SetVertices();
		SetUV();
		SetNormals();
		SetTriangles();
		_mesh.RecalculateNormals();

		while(transform.childCount > 0)
		{
			GameObjectPool.AddObjectIntoPool(transform.GetChild(0).gameObject);
		}
		if (withItems) {
			Generators[Random.Range(0, Generators.Length)].GenerateItems(this);
		}
	}

	private void SetVertices () {
		_vertices = new Vector3[PipeSegmentCount * _curveSegmentCount * 4];
		float uStep = RingDistance / _curveRadius;
		_curveAngle = uStep * _curveSegmentCount * (360f / (2f * Mathf.PI));
		CreateFirstQuadRing(uStep);
		int iDelta = PipeSegmentCount * 4;
		for (int u = 2, i = iDelta; u <= _curveSegmentCount; u++, i += iDelta) {
			CreateQuadRing(u * uStep, i);
		}
		_mesh.vertices = _vertices;
	}

	private void CreateFirstQuadRing (float u) {
		float vStep = (2f * Mathf.PI) / PipeSegmentCount;

		Vector3 vertexA = GetPointOnTorus(0f, 0f);
		Vector3 vertexB = GetPointOnTorus(u, 0f);
		for (int v = 1, i = 0; v <= PipeSegmentCount; v++, i += 4) {
			_vertices[i] = vertexA;
			_vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
			_vertices[i + 2] = vertexB;
			_vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
		}
	}

	private void CreateQuadRing (float u, int i) {
		float vStep = (2f * Mathf.PI) / PipeSegmentCount;
		int ringOffset = PipeSegmentCount * 4;

		Vector3 vertex = GetPointOnTorus(u, 0f);
		for (int v = 1; v <= PipeSegmentCount; v++, i += 4) {
			_vertices[i] = _vertices[i - ringOffset + 2];
			_vertices[i + 1] = _vertices[i - ringOffset + 3];
			_vertices[i + 2] = vertex;
			_vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
		}
	}

	void SetNormals()
	{
		_normals = new Vector3[_vertices.Length];
		for (int n = 0; n < _vertices.Length; n++)
			_normals[n] = _vertices[n].normalized;
		_mesh.normals = _normals;
	}

	private void SetUV () {
		_uv = new Vector2[_vertices.Length];
		for (int i = 0; i < _vertices.Length; i+= 4) {
			_uv[i] = Vector2.zero;
			_uv[i + 1] = Vector2.right;
			_uv[i + 2] = Vector2.up;
			_uv[i + 3] = Vector2.one;
		}
		_mesh.uv = _uv;
	}

	private void SetTriangles () {
		_triangles = new int[PipeSegmentCount * _curveSegmentCount * 6];
		for (int t = 0, i = 0; t < _triangles.Length; t += 6, i += 4) {
			_triangles[t] = i;
			_triangles[t + 1] = _triangles[t + 4] = i + 2;
			_triangles[t + 2] = _triangles[t + 3] = i + 1;
			_triangles[t + 5] = i + 3;
		}
		_mesh.triangles = _triangles;
	}

	private Vector3 GetPointOnTorus (float u, float v) {
		Vector3 p;
		float r = (_curveRadius + PipeRadius * Mathf.Cos(v));
		p.x = r * Mathf.Sin(u);
		p.y = r * Mathf.Cos(u);
		p.z = PipeRadius * Mathf.Sin(v);
		return p;
	}

	public void AlignWith (Pipe pipe) {
		_relativeRotation = Random.Range(0, _curveSegmentCount) * 360f / PipeSegmentCount;
		transform.SetParent(pipe.transform, false);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0f, 0f, -pipe._curveAngle);
		transform.Translate(0f, pipe._curveRadius, 0f);
		transform.Rotate(_relativeRotation, 0f, 0f);
		transform.Translate(0f, -_curveRadius, 0f);
		transform.SetParent(pipe.transform.parent);
		transform.localScale = Vector3.one;
	}
}