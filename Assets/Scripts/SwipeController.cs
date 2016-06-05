using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum ESwipeDirection 
{
	Up = 0,
	Right = 1,
	Down = 2,
	Left = 3
}

public class SwipeController : MonoBehaviour
{
	public static SwipeController Instance;

	[System.Serializable]
	public class SwipeEvent : UnityEvent<ESwipeDirection, float> { }

	public SwipeEvent OnSwipe;
	public bool bCanSwipe = false;

	private Vector2 firstPressPos0 = Vector2.zero;
	private Vector2 lastPressPos0 = Vector2.zero;

	private Vector2 currentSwipe = Vector2.zero;

	void Awake ()
	{
		Instance = this;
	}

	void Update()
	{
		if(bCanSwipe)
		{
#if UNITY_ANDROID || UNITY_IOS
			if (Input.touchCount > 0)
			{
				Touch t = Input.GetTouch(0);
				if (t.phase == TouchPhase.Began)
				{
					//save began touch 2d point
					firstPressPos0 = new Vector2(t.position.x, t.position.y);
				}
				if (t.phase == TouchPhase.Ended)
				{
					//save ended touch 2d point
					lastPressPos0 = new Vector2(t.position.x, t.position.y);

					//create vector from the two points
					currentSwipe = new Vector3(lastPressPos0.x - firstPressPos0.x, lastPressPos0.y - firstPressPos0.y);

					//normalize the 2d vector
					currentSwipe.Normalize();

					//swipe upwards
					if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
					{
						OnSwipe.Invoke(ESwipeDirection.Up, Input.touchCount);
					}
					//swipe down
					else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
					{
						OnSwipe.Invoke(ESwipeDirection.Down, Input.touchCount);
					}
					//swipe left
					else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
					{
						OnSwipe.Invoke(ESwipeDirection.Left, Input.touchCount);
					}
					//swipe right
					else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
					{
						OnSwipe.Invoke(ESwipeDirection.Right, Input.touchCount);
					}
				}
			}
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
			if (Input.GetMouseButtonDown(0))
			{
				//save began touch 2d point
				firstPressPos0 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			}
			if (Input.GetMouseButtonUp(0))
			{
				//save ended touch 2d point
				lastPressPos0 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

				//create vector from the two points
				currentSwipe = new Vector2(lastPressPos0.x - firstPressPos0.x, lastPressPos0.y - firstPressPos0.y);

				//normalize the 2d vector
				currentSwipe.Normalize();

				//swipe upwards
				if (currentSwipe.y > 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
				{
					OnSwipe.Invoke(ESwipeDirection.Up, 1);
				}
				//swipe down
				else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
				{
					OnSwipe.Invoke(ESwipeDirection.Down, 1);
				}
				//swipe left
				else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
				{
					OnSwipe.Invoke(ESwipeDirection.Left, 1);
				}
				//swipe right
				else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
				{
					OnSwipe.Invoke(ESwipeDirection.Right, 1);
				}
			}
#endif
		}
	}
}
