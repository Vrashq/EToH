using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public PipeSystem PipeSystem;
	public float StartVelocity;
	public float RotationVelocity;
	public MainMenu MainMenu;
	public HUD Hud;
	public float[] Accelerations;
	public Avatar Avatar;

	private Pipe _currentPipe;
	[SerializeField] private float _acceleration, _velocity;
	private float _distanceTraveled;
	private float _deltaToRotation;
	private float _systemRotation;
	private float _worldRotation, _avatarRotation;
	private Transform _world, _rotater;
	private bool _bIsGameStarted = false;

	public void StartGame (EDifficulty accelerationMode)
	{
		Hud.gameObject.SetActive(true);
		_distanceTraveled = 0f;
		_avatarRotation = 0f;
		_systemRotation = 0f;
		_worldRotation = 0f;
		_acceleration = Accelerations[(int)accelerationMode];
		_currentPipe = PipeSystem.SetupFirstPipe();
		_velocity = StartVelocity;
		SetupCurrentPipe();
		Hud.SetValues(_distanceTraveled, _velocity);
		Avatar.Mesh.localScale = Vector3.one * 0.1f;
		_bIsGameStarted = true;
	}

	public void Die () {
		MainMenu.EndGame(_distanceTraveled);
		Hud.gameObject.SetActive(false);
		gameObject.SetActive(true);
		_currentPipe = PipeSystem.SetupFirstPipe();
		_velocity = StartVelocity;
		SetupCurrentPipe();
		_bIsGameStarted = false;
	}

	public void Init ()
	{
		Hud.gameObject.SetActive(false);
		gameObject.SetActive(true);
		_currentPipe = PipeSystem.SetupFirstPipe();
		_velocity = StartVelocity;
		SetupCurrentPipe();
	}

	private void Awake () {
		_world = PipeSystem.transform.parent;
		_rotater = transform.GetChild(0);
		gameObject.SetActive(false);
	}

	private void Update ()
	{
		_velocity += _acceleration * Time.deltaTime;
		float delta = _velocity * Time.deltaTime;
		_distanceTraveled += delta;
		_systemRotation += delta * _deltaToRotation;

		if (_systemRotation >= _currentPipe.CurveAngle)
		{
			delta = (_systemRotation - _currentPipe.CurveAngle) / _deltaToRotation;
			_currentPipe = PipeSystem.SetupNextPipe(_bIsGameStarted);
			SetupCurrentPipe();
			_systemRotation = delta * _deltaToRotation;
		}

		PipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, _systemRotation);
		UpdateAvatarRotation();
		Hud.SetValues(_distanceTraveled, _velocity);
	}

	private void UpdateAvatarRotation ()
	{
		float rotationInput = 0f;
		if (_bIsGameStarted)
		{
			if (Application.isMobilePlatform)
			{
				if (Input.touchCount == 1)
				{
					if (Input.GetTouch(0).position.x < Screen.width * 0.5f)
					{
						float mouseRelativePositionX = 1.0f - Input.GetTouch(0).position.x / (Screen.width * 0.5f);
						rotationInput = -mouseRelativePositionX;
					}
					else {
						float mouseRelativePositionX = (Input.GetTouch(0).position.x - Screen.width * 0.5f) / (Screen.width * 0.5f);
						rotationInput = mouseRelativePositionX;
					}
				}
			}
			else {
				if(Input.mousePosition.x < Screen.width * 0.5f)
				{
					float mouseRelativePositionX = 1.0f - Input.mousePosition.x / (Screen.width * 0.5f);
					rotationInput = -mouseRelativePositionX;
				}
				else
				{
					float mouseRelativePositionX = (Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f);
					rotationInput = mouseRelativePositionX;
				}
			}
		}
		_avatarRotation += RotationVelocity * Time.deltaTime * rotationInput;
		if (_avatarRotation < 0f) {
			_avatarRotation += 360f;
		}
		else if (_avatarRotation >= 360f) {
			_avatarRotation -= 360f;
		}
		_rotater.localRotation = Quaternion.Euler(_avatarRotation, 0f, 0f);
	}

	private void SetupCurrentPipe () {
		_deltaToRotation = 360f / (2f * Mathf.PI * _currentPipe.CurveRadius);
		_worldRotation += _currentPipe.RelativeRotation;
		if (_worldRotation < 0f) {
			_worldRotation += 360f;
		}
		else if (_worldRotation >= 360f) {
			_worldRotation -= 360f;
		}
		_world.localRotation = Quaternion.Euler(_worldRotation, 0f, 0f);
	}
}
