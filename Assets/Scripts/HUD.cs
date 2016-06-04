﻿using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

	public Text distanceLabel, velocityLabel;

	public void SetValues (float distanceTraveled, float velocity) {
		distanceLabel.text = ((int)(distanceTraveled * 10f)).ToString();
		velocityLabel.text = ((int)(velocity * 10f)).ToString();
	}

	public void OnSwipe (ESwipeDirection direction, float amount)
	{
		if(direction == ESwipeDirection.Down)
		{
			Debug.Log("okay");
		}
	}
}