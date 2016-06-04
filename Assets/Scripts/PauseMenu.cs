using UnityEngine;
using System.Collections;

[System.Serializable]
public enum EPauseSelected
{
	Resume = 0,
	Quit = 1
}

public class PauseMenu : MonoBehaviour
{
	public RectTransform Buttons;
	public float swipeSpeed = 0.2f;
	public MainMenu mainMenu;
	public Player player;

	private Animator _animator;
	private EPauseSelected _currentSelected;
	[SerializeField] private bool _bCanTouch = false;

	public void Awake ()
	{
		RectTransform quit = Buttons.GetChild(1).GetComponent<RectTransform>();
		quit.anchoredPosition = new Vector2(Screen.width, -15.0f);
	}

	public void OnEnable ()
	{
		_animator = GetComponent<Animator>();
		_animator.SetTrigger("Animate");
		Buttons.anchoredPosition = new Vector2(0, 0);
		_currentSelected = EPauseSelected.Resume;
	}

	public void AnimationEnd ()
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
				if (_currentSelected != EPauseSelected.Quit)
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
				if (_currentSelected != EPauseSelected.Resume)
				{
					StartCoroutine(Swipe(Screen.width));
				}
				else
				{
					StartCoroutine(BadSwipeRight());
				}
			}

			switch (_currentSelected)
			{
				case EPauseSelected.Quit:
					if (direction == ESwipeDirection.Right)
					{
						_currentSelected = EPauseSelected.Resume;
					}
					break;
				case EPauseSelected.Resume:
					if (direction == ESwipeDirection.Left)
					{
						_currentSelected = EPauseSelected.Quit;
					}
					break;
			}
		}
	}

	IEnumerator Swipe(float offset)
	{
		SwipeController.Instance.bCanSwipe = false;
		float initX = Buttons.position.x;
		float timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX, initX + offset, 1.0f - timer);
			Buttons.position = new Vector3(delta, Buttons.position.y, Buttons.position.z);
			yield return null;
		}
		SwipeController.Instance.bCanSwipe = true;
	}

	IEnumerator BadSwipeRight()
	{
		SwipeController.Instance.bCanSwipe = false;
		float initX = Buttons.position.x;
		float timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX, initX + 50.0f, 1.0f - timer);
			Buttons.position = new Vector3(delta, Buttons.position.y, Buttons.position.z);
			yield return null;
		}
		timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX + 50.0f, initX, 1.0f - timer);
			Buttons.position = new Vector3(delta, Buttons.position.y, Buttons.position.z);
			yield return null;
		}

		SwipeController.Instance.bCanSwipe = true;
	}

	IEnumerator BadSwipeLeft()
	{
		SwipeController.Instance.bCanSwipe = false;
		float initX = Buttons.position.x;
		float timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX, initX - 50.0f, 1.0f - timer);
			Buttons.position = new Vector3(delta, Buttons.position.y, Buttons.position.z);
			yield return null;
		}
		timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime / swipeSpeed * 2;
			if (timer < 0)
				timer = 0;
			float delta = Mathf.Lerp(initX - 50.0f, initX, 1.0f - timer);
			Buttons.position = new Vector3(delta, Buttons.position.y, Buttons.position.z);
			yield return null;
		}

		SwipeController.Instance.bCanSwipe = true;
	}

	public void Resume ()
	{
		_bCanTouch = false;
		gameObject.SetActive(false);
		player.OnResume();
	}

	public void Quit ()
	{
		_bCanTouch = false;
		player.Die(false);
		gameObject.SetActive(false);
	}
}
