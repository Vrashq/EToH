using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public PipeSystem MyPipeSystem;
	public float StartVelocity;
	public float RotationVelocity;
	public MainMenu MainMenu;
	public HUD Hud;
	public PauseMenu PauseMenu;
	public float[] Accelerations;
	public Avatar Avatar;
	public float MaxAngleRotation;

	private Pipe _currentPipe;
	private float _acceleration, _velocity;
	private float _distanceTraveled;
	private float _deltaToRotation;
	private float _systemRotation;
	private float _worldRotation, _avatarRotation;
	private Transform _world, _rotater;
	private bool _bIsGameStarted = false;
	private bool _bIsGamePaused = false;
	private bool _bIsOnMenu = true;
	private float _ballRotation = 0;
	private float _bonusVelocity = 0;
	private EDifficulty _difficulty;


	private void Awake()
	{
		_world = MyPipeSystem.transform.parent;
		_rotater = transform.GetChild(0);
		gameObject.SetActive(false);
	}

	public void Init()
	{
		MyPipeSystem.StartSystem();
		Hud.gameObject.SetActive(false);
		gameObject.SetActive(true);
		_currentPipe = MyPipeSystem.SetupFirstPipe(true);
		_velocity = StartVelocity;
		SetupCurrentPipe();
	}

	public void OnResume ()
	{
		_bIsGamePaused = false;
		_bIsGameStarted = true;
	}

	public void AddBonus ()
	{
		switch(_difficulty)
		{
			case EDifficulty.Easy: _distanceTraveled += 25.0f; break;
			case EDifficulty.Medium: _distanceTraveled += 50.0f; break;
			case EDifficulty.Hard: _distanceTraveled += 100.0f; break;
		}
	}

	public void StartGame (EDifficulty accelerationMode)
	{
		Avatar.SetRandomColor();
		_difficulty = accelerationMode;
		_acceleration = Accelerations[(int)accelerationMode];
		Hud.gameObject.SetActive(true);
		_distanceTraveled = 0f;
		_avatarRotation = 0f;
		_systemRotation = 0f;
		_worldRotation = 0f;
		_currentPipe = MyPipeSystem.SetupFirstPipe(false);
		_velocity = StartVelocity;
		SetupCurrentPipe();
		Hud.SetValues(_distanceTraveled, _velocity);
		Avatar.Mesh.localScale = Vector3.one * 0.1f;
		_bIsGameStarted = true;
		_bIsGamePaused = false;
		_bIsOnMenu = false;
	}

	public void Die (bool sendScore = true) {
		MainMenu.EndGame(sendScore ? _distanceTraveled : 0);
		Hud.gameObject.SetActive(false);
		gameObject.SetActive(true);
		_currentPipe = MyPipeSystem.SetupFirstPipe(true);
		_velocity = StartVelocity;
		SetupCurrentPipe();
		_bIsGameStarted = false;
		_bIsGamePaused = false;
		_bIsOnMenu = true;
	}

	private void Update ()
	{
		if((!_bIsGamePaused && _bIsGameStarted) || (_bIsGameStarted && !_bIsGamePaused) || _bIsOnMenu)
		{
			_velocity += _acceleration * Time.deltaTime;
			float delta = (_velocity + _bonusVelocity) * Time.deltaTime;
			_distanceTraveled += delta;
			_systemRotation += delta * _deltaToRotation;

			if (_systemRotation >= _currentPipe.CurveAngle)
			{
				delta = (_systemRotation - _currentPipe.CurveAngle) / _deltaToRotation;
				_currentPipe = MyPipeSystem.SetupNextPipe(_bIsGameStarted);
				SetupCurrentPipe();
				_systemRotation = delta * _deltaToRotation;
			}
			MyPipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, _systemRotation);
			UpdateAvatarRotation();
			Hud.SetValues(_distanceTraveled, _velocity);
		}
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

		_ballRotation = (_ballRotation + _velocity) % 360.0f;
		Avatar.Mesh.localRotation = Quaternion.Euler(_ballRotation, 90.0f + rotationInput * MaxAngleRotation, 90.0f);
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

	public void OnSwipe (ESwipeDirection direction, float amount)
	{
		if(direction == ESwipeDirection.Down && _bIsGameStarted)
		{
			PauseMenu.gameObject.SetActive(true);
			_bIsGameStarted = false;
			_bIsGamePaused = true;
		}
	}

	public void SetColor(Color color)
	{
		MyPipeSystem.SetColor(color);
	}
}
