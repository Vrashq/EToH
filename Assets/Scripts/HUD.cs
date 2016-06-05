using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {

	public Text distanceLabel, velocityLabel;

	public void SetValues (float distanceTraveled, float velocity) {
		distanceLabel.text = "Distance: " + ((int)(distanceTraveled * 10f)).ToString();
		velocityLabel.text = "Velocity : " + ((int)(velocity * 100f) / 100f).ToString();
	}
}