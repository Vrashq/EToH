using UnityEngine;

public class PipeItem : MonoBehaviour {

	private Transform rotater;
	public Vector3 Location;
	public Quaternion Rotation;

	private void Awake () {
		rotater = transform.GetChild(0);
	}

	public void Position (Pipe pipe, float curveRotation, float ringRotation) {
		transform.parent = pipe.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
		rotater.localPosition = new Vector3(0f, pipe.CurveRadius, 0.0f);
		rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);
	}

	public void SetColor (Color color)
	{
		rotater.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", color);
	}
}