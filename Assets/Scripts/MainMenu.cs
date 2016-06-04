using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public enum EDifficulty
{
	Easy = 0,
	Medium = 1,
	Hard = 2,
	QuitGame = 3
}

public class MainMenu : MonoBehaviour
{
	private EDifficulty _currentDifficulty = EDifficulty.Easy;
	private Animator _animator;
	private bool _bCanTouch = false;

	public Player player;
	public RectTransform difficultesTransform;
	public Text[] scoreLabels;
	public float swipeSpeed = 0.2f;

	private void Awake () {
		Application.targetFrameRate = 1000;
		SaveLoad.Load();
		SetScore(scoreLabels[(int)EDifficulty.Easy], SaveLoad.savedGames[0]);
		SetScore(scoreLabels[(int)EDifficulty.Medium], SaveLoad.savedGames[1]);
		SetScore(scoreLabels[(int)EDifficulty.Hard], SaveLoad.savedGames[2]);

		_animator = GetComponent<Animator>();

		difficultesTransform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width, -15.0f);
		difficultesTransform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width * 2.0f, -15.0f);
		difficultesTransform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width * 3.0f, -15.0f);
	}

	public void OnLoadStart(float progress)
	{	
		gameObject.SetActive(true);
	}

	public void OnLoadProgress(float progress)
	{

	}

	public void OnLoadEnd(float progress)
	{
		_animator.SetTrigger("StartMenu");
		player.Init();
	}

	public void EndIntroAnimation ()
	{
		SwipeController.Instance.bCanSwipe = true;
		_bCanTouch = true;
	}

	public void OnSwipe(ESwipeDirection direction, float amount)
	{
		if(_bCanTouch)
		{
			if (direction == ESwipeDirection.Left)
			{
				if (_currentDifficulty != EDifficulty.QuitGame)
				{
					StartCoroutine(Swipe(-Screen.width));
				}
				else
				{
					StartCoroutine(BadSwipeLeft());
				}
			}

			if (direction == ESwipeDirection.Right)
			{
				if (_currentDifficulty != EDifficulty.Easy)
				{
					StartCoroutine(Swipe(Screen.width));
				}
				else
				{
					StartCoroutine(BadSwipeRight());
				}
			}

			switch (_currentDifficulty)
			{
				case EDifficulty.Easy:
					if (direction == ESwipeDirection.Left)
					{
						_currentDifficulty = EDifficulty.Medium;
					}
					break;
				case EDifficulty.Medium:
					if (direction == ESwipeDirection.Left)
					{
						_currentDifficulty = EDifficulty.Hard;
					}
					else if (direction == ESwipeDirection.Right)
					{
						_currentDifficulty = EDifficulty.Easy;
					}
					break;
				case EDifficulty.Hard:
					if(direction == ESwipeDirection.Left)
					{
						_currentDifficulty = EDifficulty.QuitGame;
					}
					else if (direction == ESwipeDirection.Right)
					{
						_currentDifficulty = EDifficulty.Medium;
					}
					break;
				case EDifficulty.QuitGame:
					if(direction == ESwipeDirection.Right)
					{
						_currentDifficulty = EDifficulty.Hard;
					}
					break;
			}
		}
	}

	IEnumerator Swipe (float offset)
	{
		SwipeController.Instance.bCanSwipe = false;
		float initX = difficultesTransform.position.x;
		float timer = 1.0f;
		while(timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX, initX + offset, 1.0f - timer);
			difficultesTransform.position = new Vector3(delta, difficultesTransform.position.y, difficultesTransform.position.z);
			yield return null;
		}
		SwipeController.Instance.bCanSwipe = true;
	}

	IEnumerator BadSwipeRight()
	{
		SwipeController.Instance.bCanSwipe = false;
		float initX = difficultesTransform.position.x;
		float timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX, initX + 50.0f, 1.0f - timer);
			difficultesTransform.position = new Vector3(delta, difficultesTransform.position.y, difficultesTransform.position.z);
			yield return null;
		}
		timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX + 50.0f, initX, 1.0f - timer);
			difficultesTransform.position = new Vector3(delta, difficultesTransform.position.y, difficultesTransform.position.z);
			yield return null;
		}

		SwipeController.Instance.bCanSwipe = true;
	}

	IEnumerator BadSwipeLeft()
	{
		SwipeController.Instance.bCanSwipe = false;
		float initX = difficultesTransform.position.x;
		float timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX, initX - 50.0f, 1.0f - timer);
			difficultesTransform.position = new Vector3(delta, difficultesTransform.position.y, difficultesTransform.position.z);
			yield return null;
		}
		timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX - 50.0f, initX, 1.0f - timer);
			difficultesTransform.position = new Vector3(delta, difficultesTransform.position.y, difficultesTransform.position.z);
			yield return null;
		}

		SwipeController.Instance.bCanSwipe = true;
	}

	public void StartGame (int mode) {
		_currentDifficulty = (EDifficulty)mode;
		player.StartGame(_currentDifficulty);
		gameObject.SetActive(false);
		_bCanTouch = false;
	}

	public void EndGame (float distanceTraveled) {
		if(distanceTraveled > SaveLoad.savedGames[(int)_currentDifficulty])
		{
			SetScore(scoreLabels[(int)_currentDifficulty], distanceTraveled);
			SaveLoad.Save(_currentDifficulty, distanceTraveled);
		}
		gameObject.SetActive(true);
		SwipeController.Instance.bCanSwipe = true;
		switch(_currentDifficulty)
		{
			case EDifficulty.Easy:		difficultesTransform.anchoredPosition = new Vector2(0, 0); break;
			case EDifficulty.Medium:	difficultesTransform.anchoredPosition = new Vector2(-Screen.width, 0); break;
			case EDifficulty.Hard:		difficultesTransform.anchoredPosition = new Vector2(-Screen.width * 2.0f, 0); break;
		}
		_bCanTouch = true;
	}

	void SetScore (Text text, float score)
	{
		text.text = "Best: " + ((int)(score * 10f)).ToString();
	}

	public void QuitGame ()
	{
		Application.Quit();
	}
}