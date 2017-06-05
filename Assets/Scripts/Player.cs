using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PanzerNoob;
using PanzerNoob.Tools;

public class Player : Actor 
{
	[AutoFinder(AutoFinder.Mode.Children, "Avatar")] public Avatar Avatar;
	[AutoFinder(AutoFinder.Mode.World)] public PipeSystem PipeSystem;
	[AutoFinder(AutoFinder.Mode.World)] public MainMenu MainMenu;
	[AutoFinder(AutoFinder.Mode.World)] public HUD Hud;
	[AutoFinder(AutoFinder.Mode.World)] public PauseMenu PauseMenu;

	public float StartVelocity;
	public float RotationVelocity;
	public float[] Accelerations;
	public float MaxAngleRotation;
	public bool bSpawnBonuses;

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


	protected virtual void OnActorAwake()
	{
		_world = PipeSystem.transform.parent;
		_rotater = transform.GetChild(0);
		gameObject.SetActive(false);
	}

	public void Init()
	{
		PipeSystem.StartSystem();
		Hud.gameObject.SetActive(false);
		gameObject.SetActive(true);
		_currentPipe = PipeSystem.SetupFirstPipe(true);
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
		_currentPipe = PipeSystem.SetupFirstPipe(false);
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
		_currentPipe = PipeSystem.SetupFirstPipe(true);
		_velocity = StartVelocity;
		SetupCurrentPipe();
		_bIsGameStarted = false;
		_bIsGamePaused = false;
		_bIsOnMenu = true;
	}

	protected virtual void OnActorUpdate ()
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
				_currentPipe = PipeSystem.SetupNextPipe(_bIsGameStarted);
				SetupCurrentPipe();
				_systemRotation = delta * _deltaToRotation;
			}
			PipeSystem.transform.localRotation = Quaternion.Euler(0f, 0f, _systemRotation);
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
		_avatarRotation %= 360.0f;
		_rotater.localRotation = Quaternion.Euler(_avatarRotation, 0f, 0f);

		_ballRotation = (_ballRotation + _velocity) % 360.0f;
		Avatar.Mesh.localRotation = Quaternion.Euler(_ballRotation, 90.0f + rotationInput * MaxAngleRotation, 90.0f);
	}

	private void SetupCurrentPipe () {
		_deltaToRotation = 360f / (2f * Mathf.PI * _currentPipe.CurveRadius);
		_worldRotation += _currentPipe.RelativeRotation;
		_worldRotation %= 360.0f;
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
		PipeSystem.SetColor(color);
	}
}
