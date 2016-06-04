using UnityEngine;
using System.Collections;

public class BonusRotator : MonoBehaviour
{
	private Transform _target;
	private float _currentRotation;

	void Start ()
	{
		_target = transform.GetChild(0);
	}

	void Update ()
	{
		_currentRotation += Time.deltaTime * 180.0f;
		_target.rotation = Quaternion.Euler(0, _currentRotation, 0);
	}
}
